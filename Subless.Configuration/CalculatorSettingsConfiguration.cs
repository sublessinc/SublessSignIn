using Microsoft.Extensions.DependencyInjection;
using Subless.PayoutCalculator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Configuration
{
    public static class CalculatorSettingsConfiguration
    {
        public static IServiceCollection RegisterCalculatorConfig(IServiceCollection services)
        {
            services.Configure<CalculatorConfiguration>(options =>
            {
                options.BucketName = Environment.GetEnvironmentVariable("BucketName") ?? throw new ArgumentNullException("BucketName");
                options.ExecutionsPerYear = int.Parse(Environment.GetEnvironmentVariable("ExecutionsPerYear") ?? throw new ArgumentNullException("ExecutionsPerYear"), CultureInfo.InvariantCulture);
                options.RunOnStart = bool.Parse(Environment.GetEnvironmentVariable("RunOnStart") ?? "false");
                options.CalcuationRangeEnd = Environment.GetEnvironmentVariable("CalcuationRangeEnd");
                options.CalcuationRangeStart = Environment.GetEnvironmentVariable("CalcuationRangeStart");
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN");
                options.PoolId = Environment.GetEnvironmentVariable("DOMAIN");
            });
            return services;
        }
    }
}
