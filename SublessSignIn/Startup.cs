using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Subless.Configuration;
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
            AuthSettings = AuthSettingsConfiguration.GetAuthSettings();

        }


        public AuthSettings AuthSettings { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //The order of these two auth schemes matters. The last one added will be the default, so we add the partner facing bearer token scheme first.
            AuthSettingsConfiguration.RegisterAuthSettingsConfig(services, AuthSettings);
            StripeConfiguration.RegisterStripeConfig(services);
            CalculatorSettingsConfiguration.RegisterCalculatorConfig(services);
            GeneralConfiguration.RegisterGeneralConfig(services);
            services = new BearerAuth().AddBearerAuthServices(services, AuthSettings);
            services.AddTransient<IHealthCheck, HealthCheck>();
            services.AddTransient<IVersion, FileVersion>();
            services.AddBffServices(AuthSettings);
            services.RegisterAuthDi(AuthSettings);
            services.AddMiniProfiler().AddEntityFramework();

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
                      Array.Empty<string>()
                    }
                  });
            });
            services.AddMvc();
            if (false) // disable miniprofiler
            {
                services.AddMiniProfiler(options =>
                {
                    options.RouteBasePath = "/profiler";
                    options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
                    options.TrackConnectionOpenClose = true;
                    options.EnableMvcFilterProfiling = true;
                    options.EnableMvcViewProfiling = true;
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Hosting.IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopping.Register(OnStopping);
            applicationLifetime.ApplicationStopped.Register(OnStopped);
            if (false) // disable miniprofiler
            {
                app.UseMiniProfiler();
            }

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
            app.UseMiddleware<ExceptionHandlingMiddleware>();
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
