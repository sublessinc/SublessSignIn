using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Subless.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SublessSignIn.Models;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using Subless.Services;
using Subless.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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
            AuthSettings.IssuerUrl = "https://auth.subless.com";
            AuthSettings.CognitoUrl = $"https://cognito-idp.{AuthSettings.Region}.amazonaws.com/{AuthSettings.PoolId}";
            AuthSettings.JwtKeySetUrl = AuthSettings.CognitoUrl + "/.well-known/jwks.json";

        }


        public AuthSettings AuthSettings { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<StripeConfig>(options =>
            {
                options.PublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY") ?? throw new ArgumentNullException("STRIPE_PUBLISHABLE_KEY");
                options.SecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") ?? throw new ArgumentNullException("STRIPE_SECRET_KEY");
                options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET") ?? throw new ArgumentNullException("STRIPE_WEBHOOK_SECRET");
                options.BasicPrice = Environment.GetEnvironmentVariable("BASIC_PRICE_ID") ?? throw new ArgumentNullException("BASIC_PRICE_ID");
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? throw new ArgumentNullException("DOMAIN");
            });
            services.Configure<DatabaseSettings>(options =>
            {
                options.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new ArgumentNullException("CONNECTION_STRING");
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

            // TODO: See if mars catches this in code review, and also see if we can restrict this 
            services.AddCors(o => o.AddPolicy("Unrestricted", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<UserRepository, UserRepository>();
            services.AddTransient<IAdministrationService, AdministrationService>();
            services.AddTransient<IPartnerService, PartnerService>();
            services.AddTransient<ICreatorService, CreatorService>();
            services.AddTransient<IHitService, HitService>();
            services.AddTransient<IUserService, UserService>();
            services.AddDbContext<UserRepository>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SublessSignIn", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
            });


            // Add Authentication services

            services.AddAuthentication("Bearer")
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = GetCognitoTokenValidationParams();
                 });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SublessSignIn v1"));
            }

            //app.UseHttpsRedirection();

            app.UseFileServer();
            app.UseAuthentication();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");
            });
        }


        private TokenValidationParameters GetCognitoTokenValidationParams()
        {
            return new TokenValidationParameters
            {
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    // get JsonWebKeySet from AWS
                    var json = new WebClient().DownloadString(AuthSettings.JwtKeySetUrl);

                    // serialize the result
                    var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;

                    // cast the result to be the type expected by IssuerSigningKeyResolver
                    return (IEnumerable<SecurityKey>)keys;
                },
                ValidIssuer = AuthSettings.CognitoUrl,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                AudienceValidator = AudienceValidator,
            };
        }

        public bool AudienceValidator(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (securityToken is JwtSecurityToken)
            {
                var token = (JwtSecurityToken)securityToken;
                var tokenUse = token.Claims.FirstOrDefault(x=> x.Type == "token_use")?.Value;
                if (tokenUse == null)
                {
                    return false;
                }
                if (tokenUse == "access")
                {
                    return true;
                }
                if (tokenUse == "id")
                {
                    return AuthSettings.AppClientId == audiences.First();
                }
            }
            return false;
        }

    }
}