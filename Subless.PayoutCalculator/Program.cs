using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Subless.Data;
using Subless.PayoutCalculator;
using Subless.Services;

namespace PayoutCalculator
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var calculator = scope.ServiceProvider.GetRequiredService<ICalculatorService>();
                calculator.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
            }
            return host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.Configure<AwsConfiguration>(options =>
                {
                    options.AccessKey = Environment.GetEnvironmentVariable("AccessKey") ?? throw new ArgumentNullException("AccessKey");
                    options.SecretKey = Environment.GetEnvironmentVariable("SecretKey") ?? throw new ArgumentNullException("SecretKey");
                });
                DataDi.RegisterDataDi(services);
                ServicesDi.AddServicesDi(services);
                services.AddTransient<ICalculatorService, CalculatorService>();
                services.AddTransient<IS3Service, S3Service>();
                services.AddTransient<AWSCredentials, AwsCredWrapper>();

            });
        }
    }
}
