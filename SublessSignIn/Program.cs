using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Subless.Data;
using Subless.Services;
using SublessSignIn.AuthServices;

namespace SublessSignIn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            Log.Logger.Information("Subless bootstrapping...");

            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetService<UserRepository>())
                {
                    Log.Logger.Information("Running migrations");
                    context.Database.Migrate();
                    context.SaveChanges();
                    context.LogDbStats();
                }
                var adminService = services.GetService<IAdministrationService>();
                adminService.OutputAdminKeyIfNoAdmins();
                var cors = services.GetService<ICorsPolicyAccessor>();
                cors.SetUnrestrictedOrigins();
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:7070/", "https://0.0.0.0:7071/");
                });
        }
    }
}
