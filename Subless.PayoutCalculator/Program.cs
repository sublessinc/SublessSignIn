using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Subless.Data;
using Subless.PayoutCalculator;
using Subless.Services;
using Subless.Services.Services;

namespace PayoutCalculator
{
    internal class Program
    {
        private async static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                // initialize configuration
                var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
                var logging_env_var = System.Environment.GetEnvironmentVariable("Logging__LogLevel__Default");
                logger.LogInformation($"Logging env var value is {logging_env_var}");
                try {
                    logger.LogDebug("Logging in debug mode.");
                    var healthCheck = scope.ServiceProvider.GetRequiredService<IHealthCheck>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IOptions<CalculatorConfiguration>>();
                    var shouldExecuteOnStart = configuration.Value.RunOnStart;
                    var executionsPerYear = configuration.Value.ExecutionsPerYear;
                    if (shouldExecuteOnStart) { logger.LogError("EXECUTING CALUCLATOR ON START! THIS IS A TESTING FEATURE AND SHOULD NOT EXECUTE IN PRODUCTION."); }

                    if (!(await healthCheck.IsHealthy()))
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
                        
                        if (ShouldExecuteScheduledRun(executionsPerYear, lastExecution) || shouldExecuteOnStart)
                        {
                            shouldExecuteOnStart = false;
                            logger.LogInformation("Running calculation");
                            calculator.CalculatePayments(lastExecution, DateTimeOffset.UtcNow);
                            logger.LogInformation("Calculation complete");
                        }
                        Thread.Sleep(1000 * 60);
                    }
                } catch (Exception e) {
                    logger.LogCritical(e, "Critical unhandled exception in calculator. Shutting down. Good luck.");
                }
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .UseSerilog(LoggerConfig.GetLogger())
            .ConfigureServices((_, services) =>
            {
                services.Configure<CalculatorConfiguration>(options =>
                {
                    options.BucketName = Environment.GetEnvironmentVariable("BucketName") ?? throw new ArgumentNullException("BucketName");
                    options.ExecutionsPerYear = int.Parse( Environment.GetEnvironmentVariable("ExecutionsPerYear") ?? throw new ArgumentNullException("ExecutionsPerYear"));
                    options.RunOnStart = bool.Parse(Environment.GetEnvironmentVariable("RunOnStart") ?? throw new ArgumentNullException("RunOnStart"));
                });
                DataDi.RegisterDataDi(services);
                ServicesDi.AddServicesDi(services);
                services.AddTransient<ICalculatorService, CalculatorService>();
                services.AddTransient<IFileStorageService, S3Service>();
                services.AddTransient<AwsCredWrapper, AwsCredWrapper>();
                services.AddTransient<IHealthCheck, HealthCheck>();

            });
        }

        private static bool ShouldExecuteScheduledRun(int execPerYear, DateTimeOffset lastRunDate)
        {
            var secondsPerYear = TimeSpan.FromDays(365).TotalSeconds;
            var secondsBetweenRuns = secondsPerYear / execPerYear;
            return lastRunDate.AddSeconds(secondsBetweenRuns) < DateTimeOffset.UtcNow;
        }
    }
}
