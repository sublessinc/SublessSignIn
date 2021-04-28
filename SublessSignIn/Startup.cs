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

namespace SublessSignIn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthSettings = new AuthSettings
            {
                Region = Environment.GetEnvironmentVariable("region"),
                PoolId = Environment.GetEnvironmentVariable("userPoolId"),
                AppClientId = Environment.GetEnvironmentVariable("appClientId")
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
                options.PublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
                options.SecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
                options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
                options.BasicPrice = Environment.GetEnvironmentVariable("BASIC_PRICE_ID");
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN");
            });
            services.Configure<DatabaseSettings>(options =>
            {
                options.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
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
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<UserRepository, UserRepository>();
            services.AddDbContext<UserRepository>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SublessSignIn", Version = "v1" });
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

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
                ValidAudience = AuthSettings.AppClientId
            };
        }
    }
}