using Microsoft.Extensions.DependencyInjection;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            });
            return services;
        }
    }
}
