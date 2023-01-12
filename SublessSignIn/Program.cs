using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Subless.Data;
using Subless.Services.Services;
using SublessSignIn.AuthServices;

namespace SublessSignIn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = LoggerConfig.GetLogger();
            Log.Logger.Information("Subless bootstrapping...");

            var host = CreateHostBuilder(args, Log.Logger).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetService<Repository>())
                {

                    Log.Logger.Information("Running migrations");
                    context.Database.Migrate();
                    context.SaveChanges();
                    context.LogDbStats();
                }
                using (var context = services.GetService<KeyStorageContext>())
                {
                    Log.Logger.Information("Running data protection migrations");
                    context.Database.Migrate();
                    context.SaveChanges();
                }
                var adminService = services.GetService<IAdministrationService>();
                adminService.OutputAdminKeyIfNoAdmins();
                var cors = services.GetService<ICorsPolicyAccessor>();
                cors.SetUnrestrictedOrigins();
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, ILogger logger)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog(logger)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10); });
                    webBuilder.UseUrls("http://0.0.0.0:7070/");
                    webBuilder.UseShutdownTimeout(TimeSpan.FromSeconds(2));
                });
        }


    }
}
