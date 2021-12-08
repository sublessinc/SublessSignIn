using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Duende.Bff;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Subless.Data;
using Subless.Models;
using Subless.Services;
using SublessSignIn.AuthServices;

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
            AuthSettings.Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? throw new ArgumentNullException("DOMAIN");

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



            services.Configure<AuthSettings>(options =>
            {
                options.AppClientId = AuthSettings.AppClientId;
                options.CognitoUrl = AuthSettings.CognitoUrl;
                options.IssuerUrl = AuthSettings.IssuerUrl;
                options.JwtKeySetUrl = AuthSettings.JwtKeySetUrl;
                options.PoolId = AuthSettings.PoolId;
                options.Region = AuthSettings.Region;
            });


            services.AddBffServices(AuthSettings);


            DataDi.RegisterDataDi(services);

            // TODO: See if mars catches this in code review, and also see if we can restrict this 
            services.AddCors(o => o.AddPolicy(CorsPolicyAccessor.UnrestrictedPolicy, builder =>
            {
                builder.WithOrigins()
                       .AllowCredentials()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            ServicesDi.AddServicesDi(services);
            services.AddTransient<ILoginService, SublessLoginService>();
            services.AddTransient<ICorsPolicyAccessor, CorsPolicyAccessor>();



            //services.Replace(descriptor2);
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
            app.UseForwardedHeaders();
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
    }
}