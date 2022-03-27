using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Subless.Data;
using Subless.Models;
using Subless.Services;
using SublessSignIn.AuthServices;
using System;
using System.Linq;
using static Subless.Data.DataDi;

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
            AuthSettings.IdentityServerLicenseKey = Environment.GetEnvironmentVariable("IdentityServerLicenseKey") ?? "";
            var json = Environment.GetEnvironmentVariable("dbCreds") ?? throw new ArgumentNullException("dbCreds");
            var dbCreds = JsonConvert.DeserializeObject<DbCreds>(json);
            AuthSettings.SessionStoreConnString = dbCreds.GetDatabaseConnection();
            if (!AuthSettings.Domain.EndsWith('/'))
            {
                AuthSettings.Domain += '/';
            }

        }


        public AuthSettings AuthSettings { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //The order of these two auth schemes matters. The last one added will be the default, so we add the partner facing bearer token scheme first.
            services = new BearerAuth().AddBearerAuthServices(services, AuthSettings);
            services.AddTransient<IHealthCheck, HealthCheck>();
            services.AddTransient<IVersion, FileVersion>();
            services.AddBffServices(AuthSettings);
            services.RegisterAuthDi(AuthSettings);

            DataDi.RegisterDataDi(services);

            services.AddCors(o => o.AddPolicy(CorsPolicyAccessor.UnrestrictedPolicy, builder =>
            {
                builder.WithOrigins()
                       .AllowCredentials()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            ServicesDi.AddServicesDi(services);

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Hosting.IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopping.Register(OnStopping);
            applicationLifetime.ApplicationStopped.Register(OnStopped);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                applicationLifetime.StopApplication();
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
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

        private void OnStarted()
        {
            Console.WriteLine("Started");
        }

        private void OnStopping()
        {
            Console.WriteLine("Stopping");
        }

        private void OnStopped()
        {
            Console.WriteLine("Stopped");
        }
    }
}