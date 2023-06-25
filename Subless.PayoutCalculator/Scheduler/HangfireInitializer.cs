using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Data;
using System;

namespace Subless.PayoutCalculator.Scheduler
{
    public static class HangfireInitializer
    {
        public static void StartHangfire(IServiceScope scope, ILogger logger, IOptions<CalculatorConfiguration> configuration)
        {
            var dbConfig = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();

            GlobalConfiguration.Configuration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseColouredConsoleLogProvider()
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UsePostgreSqlStorage(dbConfig.Value.ConnectionString);

            logger.LogInformation($"Setting reminder emails on schedule {configuration.Value.ReminderEmailSchedule}");
            RecurringJob.AddOrUpdate<IReminderEmailJob>("ReminderEmail", x => x.QueueIdleEmailsForThisMonth(), configuration.Value.ReminderEmailSchedule);
        }

        public static BackgroundJobServer GetJobServer(IServiceProvider serviceProvider)
        {
            return new BackgroundJobServer(new BackgroundJobServerOptions { Activator = new DependencyJobActivator(serviceProvider) });
        }
    }
}
