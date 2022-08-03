using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services.Services.SublessStripe;

namespace Subless.Configuration
{
    public class StripeConfiguration
    {
        public static IServiceCollection RegisterStripeConfig(IServiceCollection services)
        {
            services.Configure<StripeConfig>(options =>
            {
                options.PublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY") ?? throw new ArgumentNullException("STRIPE_PUBLISHABLE_KEY");
                options.SecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") ?? throw new ArgumentNullException("STRIPE_SECRET_KEY");
                options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET") ?? throw new ArgumentNullException("STRIPE_WEBHOOK_SECRET");
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? throw new ArgumentNullException("DOMAIN");
                if (!options.Domain.EndsWith('/'))
                {
                    options.Domain += '/';
                }
                options.SublessPayPalId = Environment.GetEnvironmentVariable("PayPalId");
                options.CustomBudgetId = Environment.GetEnvironmentVariable("CustomBudgetId");
            });

            var serviceProvider = services.BuildServiceProvider();
            StripeApiWrapperServiceFactory.StripeConfig = serviceProvider.GetService<IOptions<StripeConfig>>();
            return services;
        }
    }
}
