using Microsoft.Extensions.DependencyInjection;
using Subless.PayoutCalculator;

namespace Subless.Configuration
{
    public static class CalculatorSettingsConfiguration
    {
        public static IServiceCollection RegisterCalculatorConfig(IServiceCollection services)
        {
            services.Configure<CalculatorConfiguration>(options =>
            {
                options.BucketName = Environment.GetEnvironmentVariable("BucketName") ?? throw new ArgumentNullException("BucketName");
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN");
                options.PoolId = Environment.GetEnvironmentVariable("PoolId");
                options.ReminderEmailSchedule = Environment.GetEnvironmentVariable("ReminderEmailSchedule") ?? throw new ArgumentNullException("ReminderEmailSchedule"); ;
            });
            return services;
        }
    }
}
