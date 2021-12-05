using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Duende.Bff;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Subless.Data;
using Subless.Models;
using Subless.Services;

namespace SublessSignIn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthSettings = new AuthSettings
            {
                Region = Environment.GetEnvironmentVariable("region") ?? throw new ArgumentNullException("region"),
                PoolId = Environment.GetEnvironmentVariable("userPoolId") ?? throw new ArgumentNullException("userPoolId"),
                AppClientId = Environment.GetEnvironmentVariable("appClientId") ?? throw new ArgumentNullException("appClientId")
            };
            AuthSettings.IssuerUrl = Environment.GetEnvironmentVariable("issuerUrl") ?? throw new ArgumentNullException("issuerUrl");
            AuthSettings.CognitoUrl = $"https://cognito-idp.{AuthSettings.Region}.amazonaws.com/{AuthSettings.PoolId}";
            AuthSettings.JwtKeySetUrl = AuthSettings.CognitoUrl + "/.well-known/jwks.json";

        }


        public AuthSettings AuthSettings { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddBff()
                .AddServerSideSessions();
            // configure server-side authentication and session management
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
                .AddCookie("cookie", options =>
                {
                // host prefixed cookie name
                options.Cookie.Name = "__Host-spa";

                // strict SameSite handling
                options.Cookie.SameSite = SameSiteMode.None;
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = AuthSettings.CognitoUrl;

                // confidential client using code flow + PKCE + query response mode
                options.ClientId = AuthSettings.AppClientId;
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.ResponseMode = "query";
                    options.UsePkce = true;
                    options.MapInboundClaims = false;
                    options.GetClaimsFromUserInfoEndpoint = true;

                // save access and refresh token to enable automatic lifetime management
                options.SaveTokens = true;

                // request scopes
                options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    //options.Scope.Add("api");

                    // request refresh token
                    //options.Scope.Add("offline_access");
                });

            services.Configure<AuthSettings>(options =>
            {
                options.AppClientId = AuthSettings.AppClientId;
                options.CognitoUrl = AuthSettings.CognitoUrl;
                options.IssuerUrl = AuthSettings.IssuerUrl;
                options.JwtKeySetUrl = AuthSettings.JwtKeySetUrl;
                options.PoolId = AuthSettings.PoolId;
                options.Region = AuthSettings.Region;
            });

            DataDi.RegisterDataDi(services);

            // TODO: See if mars catches this in code review, and also see if we can restrict this 
            services.AddCors(o => o.AddPolicy("Unrestricted", builder =>
            {
                builder.WithOrigins("http://localhost:5000","http://localhost")
                        .AllowCredentials()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            ServicesDi.AddServicesDi(services);
            services.AddTransient<ILoginService, SublessLoginService>();
            services.AddControllers();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SublessSignIn", Version = "v1" });
            //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        In = ParameterLocation.Header,
            //        Description = "Please insert JWT with Bearer into field",
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey
            //    });
            //    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            //       {
            //         new OpenApiSecurityScheme
            //         {
            //           Reference = new OpenApiReference
            //           {
            //             Type = ReferenceType.SecurityScheme,
            //             Id = "Bearer"
            //           }
            //          },
            //          new string[] { }
            //        }
            //      });
            //});


            // Add Authentication services

            //services.AddAuthentication("Bearer")
            //     .AddJwtBearer(options =>
            //     {
            //         options.TokenValidationParameters = GetCognitoTokenValidationParams();
            //     });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SublessSignIn v1"));
            }
            app.UseForwardedHeaders();

            // I dont think we need this, however, I don't fully understand it, so restore it if the front end file server starts acting a fool - 9/27/21
            //app.UseFileServer();
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                OnPrepareResponse = (ctx) =>
                {
                    // for some reason, we have to override the headers for static files
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                      "Origin, X-Requested-With, Content-Type, Accept");
                },
            });
            app.UseAuthentication();


            // adds antiforgery protection for local APIs
            app.UseBff();

            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()                 
                    .RequireAuthorization()
                    .AsBffApiEndpoint();

                // login, logout, user, backchannel logout...
                endpoints.MapBffManagementEndpoints();
                endpoints.MapFallbackToFile("/index.html");
            });
        }


        //private TokenValidationParameters GetCognitoTokenValidationParams()
        //{
        //    return new TokenValidationParameters
        //    {
        //        IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
        //        {
        //            // get JsonWebKeySet from AWS
        //            var json = new WebClient().DownloadString(AuthSettings.JwtKeySetUrl);

        //            // serialize the result
        //            var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;

        //            // cast the result to be the type expected by IssuerSigningKeyResolver
        //            return keys;
        //        },
        //        ValidIssuer = AuthSettings.CognitoUrl,
        //        ValidateIssuerSigningKey = true,
        //        ValidateIssuer = true,
        //        ValidateLifetime = true,
        //        AudienceValidator = AudienceValidator,
        //    };
        //}

        //public bool AudienceValidator(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        //{
        //    if (securityToken is JwtSecurityToken)
        //    {
        //        var token = (JwtSecurityToken)securityToken;
        //        var tokenUse = token.Claims.FirstOrDefault(x => x.Type == "token_use")?.Value;
        //        if (tokenUse == null)
        //        {
        //            return false;
        //        }
        //        if (tokenUse == "access")
        //        {
        //            return true;
        //        }
        //        if (tokenUse == "id")
        //        {
        //            return AuthSettings.AppClientId == audiences.First();
        //        }
        //    }
        //    return false;
        //}

    }
    public class SublessLoginService : ILoginService
    {
        private readonly BffOptions _options;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options"></param>
        public SublessLoginService(BffOptions options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public async Task ProcessRequestAsync(HttpContext context)
        {

            var returnUrl = context.Request.Query[Constants.RequestParameters.ReturnUrl].FirstOrDefault();

            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/"
            };

            await context.ChallengeAsync(props);
        }
    }
}