using Microsoft.Extensions.DependencyInjection;
using Subless.Models;

namespace Subless.Configuration
{
    public class GeneralConfiguration
    {
        public static IServiceCollection RegisterGeneralConfig(IServiceCollection services)
        {
            services.Configure<GeneralConfig>(options =>
            {
                options.Environment = Environment.GetEnvironmentVariable("EnvironmentName") ?? throw new ArgumentNullException("EnvironmentName");
            });
            return services;
        }
    }
}
