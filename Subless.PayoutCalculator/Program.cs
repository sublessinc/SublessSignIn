using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Subless.Data;
using Subless.PayoutCalculator;
using Subless.Services;
using Subless.Services.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PayoutCalculator
{
    internal class Program
    {
        public static Microsoft.Extensions.Logging.ILogger logger;
        private async static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                // initialize configuration
                logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
                var logging_env_var = System.Environment.GetEnvironmentVariable("Logging__LogLevel__Default");
                logger.LogInformation($"Logging env var value is {logging_env_var}");
                try
                {
                    logger.LogDebug("Logging in debug mode.");
                    var healthCheck = scope.ServiceProvider.GetRequiredService<IHealthCheck>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IOptions<CalculatorConfiguration>>();


                    if (!(await healthCheck.IsHealthy()))
                    {
                        throw new Exception("Could not start due to health check failure");
                    }
                    logger.LogInformation("All dependencies responding, starting services");
                    RunCalculator(configuration.Value, host);

                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "Critical unhandled exception in calculator. Shutting down. Good luck.");
                }
            }
        }

        private static void RunCalculator(CalculatorConfiguration configuration, IHost host)
        {
            if (configuration.RunOnStart)
            {
                logger.LogError("EXECUTING CALUCLATOR ON START! THIS IS A TESTING FEATURE AND SHOULD NOT EXECUTE IN PRODUCTION.");
                CalculateAllPaymentsSinceLastRunAndStop(host);
            }
            else if (DateTime.TryParse(configuration.CalcuationRangeEnd, out DateTime end) && DateTime.TryParse(configuration.CalcuationRangeStart, out DateTime start))
            {
                logger.LogError("EXECUTING CALUCLATOR OVER PRE-SET-RANGE! THIS IS A TESTING FEATURE AND SHOULD NOT EXECUTE IN PRODUCTION");
                RunOverSpecifedRange(host, start, end);
            }
            else
            {
                var executionsPerYear = configuration.ExecutionsPerYear;
                RunOnInfiniteLoop(host, executionsPerYear);
            }
        }

        private static void RunOnInfiniteLoop(IHost host, int executionsPerYear)
        {
            using (var scope = host.Services.CreateScope())
            {
                while (true)
                {
                    logger.LogInformation("Checking if calculator should be run");
                    var logsService = scope.ServiceProvider.GetRequiredService<IPaymentLogsService>();
                    var calculator = scope.ServiceProvider.GetRequiredService<ICalculatorService>();
                    var lastExecution = logsService.GetLastPaymentDate();

                    if (ShouldExecuteScheduledRun(executionsPerYear, lastExecution))
                    {
                        logger.LogInformation("Running calculation");
                        calculator.CalculatePayments(lastExecution, DateTimeOffset.UtcNow);
                        logger.LogInformation("Calculation complete");
                    }
                    Thread.Sleep(1000 * 60);
                }
            }
        }

        private static void CalculateAllPaymentsSinceLastRunAndStop(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var logsService = scope.ServiceProvider.GetRequiredService<IPaymentLogsService>();
                var calculator = scope.ServiceProvider.GetRequiredService<ICalculatorService>();
                var lastExecution = logsService.GetLastPaymentDate();
                logger.LogInformation("Running calculation");
                calculator.CalculatePayments(lastExecution, DateTimeOffset.UtcNow);
                logger.LogError("Calculation complete... waiting indefinitly");
                Console.Read();
            }
        }

        private static void RunOverSpecifedRange(IHost host, DateTime start, DateTime end)
        {
            using (var scope = host.Services.CreateScope())
            {
                var logsService = scope.ServiceProvider.GetRequiredService<IPaymentLogsService>();
                var calculator = scope.ServiceProvider.GetRequiredService<ICalculatorService>();
                var lastExecution = logsService.GetLastPaymentDate();
                logger.LogInformation("Running calculation");
                calculator.CalculatePayments(start, end);
                logger.LogError("Calculation complete... waiting indefinitly");
                Console.Read();
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
                    options.ExecutionsPerYear = int.Parse(Environment.GetEnvironmentVariable("ExecutionsPerYear") ?? throw new ArgumentNullException("ExecutionsPerYear"));
                    options.RunOnStart = bool.Parse(Environment.GetEnvironmentVariable("RunOnStart") ?? throw new ArgumentNullException("RunOnStart"));
                    options.CalcuationRangeEnd = Environment.GetEnvironmentVariable("CalcuationRangeEnd");
                    options.CalcuationRangeStart = Environment.GetEnvironmentVariable("CalcuationRangeStart");
                    options.Domain = Environment.GetEnvironmentVariable("DOMAIN");
                    options.PoolId = Environment.GetEnvironmentVariable("DOMAIN");
                });
                DataDi.RegisterDataDi(services);
                ServicesDi.AddServicesDi(services);
                services.AddTransient<ICalculatorService, CalculatorService>();
                services.AddTransient<IFileStorageService, S3Service>();
                services.AddTransient<AwsCredWrapper, AwsCredWrapper>();
                services.AddTransient<IEmailService, EmailService>();
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

