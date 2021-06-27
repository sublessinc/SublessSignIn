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
    class Program
    {
        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var calculator = scope.ServiceProvider.GetRequiredService<ICalculatorService>();
                calculator.CalculatePayments(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
            }
            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                services.Configure<AwsCreds>(options =>
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
