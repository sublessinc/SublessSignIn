﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Subless.Configuration;
using Subless.Data;
using Subless.PayoutCalculator;
using Subless.PayoutCalculator.Scheduler;
using Subless.Services;
using Subless.Services.Services;
using Subless.Services.Services.SublessStripe;

namespace PayoutCalculator
{
    internal class Program
    {
        public static Microsoft.Extensions.Logging.ILogger logger;
        private async static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                // initialize configuration
                logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
                try
                {
                    var version = System.IO.File.ReadAllText(@"version.txt");
                    logger.LogInformation("Version: " + version);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Unable to find the version.txt file for printing our version.");
                }

                var logging_env_var = System.Environment.GetEnvironmentVariable("Logging__LogLevel__Default");
                logger.LogInformation($"Logging env var value is {logging_env_var}");



                try
                {
                    logger.LogDebug("Logging in debug mode.");
                    var healthCheck = scope.ServiceProvider.GetRequiredService<IHealthCheck>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IOptions<CalculatorConfiguration>>();

                    logger.LogDebug("Setting up hangfire");

                    HangfireInitializer.StartHangfire(scope, logger, configuration);
                    
                    if (!await healthCheck.IsHealthy())
                    {
                        throw new HealthCheckFailureException("Could not start due to health check failure");
                    }
                    logger.LogInformation("All dependencies responding, starting services");
                    using (var server = HangfireInitializer.GetJobServer(scope.ServiceProvider))
                    {                    
                        RunCalculator(configuration.Value, host);
                    }

                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "Critical unhandled exception in calculator. Shutting down. Good luck.");
                }
            }
        }

        private static void RunCalculator(CalculatorConfiguration configuration, IHost host)
        {
            while (true)
            {

                using (var scope = host.Services.CreateScope())
                {
                    logger.LogInformation("Checking if calculator should be run");
                    var calculatorService = scope.ServiceProvider.GetRequiredService<ICalculatorService>();
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                    calculatorService.ExecutedQueuedCalculation();
                    paymentService.ExecutedQueuedPayment();
                    paymentService.ExecuteQueuedIdleEmail();
                    paymentService.ExecuteStripeSync();
                    Thread.Sleep(1000 * 10);
                }
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .UseSerilog(LoggerConfig.GetLogger())
            .ConfigureServices((_, services) =>
            {
                StripeConfiguration.RegisterStripeConfig(services);
                CalculatorSettingsConfiguration.RegisterCalculatorConfig(services);
                GeneralConfiguration.RegisterGeneralConfig(services);
                var authSettings = AuthSettingsConfiguration.GetAuthSettings();
                AuthSettingsConfiguration.RegisterAuthSettingsConfig(services, authSettings);
                BffDi.AddBffDi(services, authSettings);
                DataDi.RegisterDataDi(services);
                ServicesDi.AddServicesDi(services);
                services.AddTransient<IHealthCheck, HealthCheck>();
                services.AddTransient<IReminderEmailJob, ReminderEmailJob>();
            });
        }
    }
}

