using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Subless.Services.Services;

namespace Subless.Services
{
    public static class CalculatorDi
    {
        public static IServiceCollection AddCalculatorDi(IServiceCollection services)
        {
            services.AddTransient<ICalculatorService, CalculatorService>();
            services.AddTransient<IFileStorageService, S3Service>();
            services.AddTransient<AwsCredWrapper, AwsCredWrapper>();
            services.AddTransient<IPaymentEmailService, PaymentEmailService>();
            services.AddTransient<IHealthCheck, HealthCheck>();
            return services;
        }
    }
}
