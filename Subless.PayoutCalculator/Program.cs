using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.PayoutCalculator;
using Subless.Services;

namespace PayoutCalculator
{
    internal class Program
    {
        private async static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
                var healthCheck = scope.ServiceProvider.GetRequiredService<IHealthCheck>();
                if (!(await healthCheck.IsHealthly()))
                {
                    throw new Exception("Could not start due to health check failure");
                }
                logger.LogInformation("All dependencies responding, starting services");
                while (true)
                {
                    logger.LogInformation("Checking if calculator should be run");
                    var logsService = scope.ServiceProvider.GetRequiredService<IPaymentLogsService>();
                    var calculator = scope.ServiceProvider.GetRequiredService<ICalculatorService>();
                    var lastExecution = logsService.GetLastPaymentDate();
                    if (lastExecution < DateTime.UtcNow.AddMonths(-1))
                    {
                        logger.LogInformation("Running calculation");
                        calculator.CalculatePayments(lastExecution, DateTime.UtcNow);
                        logger.LogInformation("Calculation complete");
                    }
                    Thread.Sleep(1000 * 60);
                }
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.Configure<AwsConfiguration>(options =>
                {
                    options.BucketName = Environment.GetEnvironmentVariable("BucketName") ?? throw new ArgumentNullException("BucketName");
                });
                DataDi.RegisterDataDi(services);
                ServicesDi.AddServicesDi(services);
                services.AddTransient<ICalculatorService, CalculatorService>();
                services.AddTransient<IFileStorageService, S3Service>();
                services.AddTransient<AwsCredWrapper, AwsCredWrapper>();
                services.AddTransient<IHealthCheck, HealthCheck>();

            });
        }
    }
}
