using System;
using Microsoft.Extensions.DependencyInjection;
using Subless.Models;
using Subless.Services.Services;

namespace Subless.Services
{
    public static class ServicesDi
    {
        public static IServiceCollection AddServicesDi(IServiceCollection services)
        {
            services.Configure<StripeConfig>(options =>
            {
                options.PublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY") ?? throw new ArgumentNullException("STRIPE_PUBLISHABLE_KEY");
                options.SecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") ?? throw new ArgumentNullException("STRIPE_SECRET_KEY");
                options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET") ?? throw new ArgumentNullException("STRIPE_WEBHOOK_SECRET");
                options.BasicPrice = Environment.GetEnvironmentVariable("BASIC_PRICE_ID") ?? throw new ArgumentNullException("BASIC_PRICE_ID");
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? throw new ArgumentNullException("DOMAIN");
                options.SublessPayoneerId = Environment.GetEnvironmentVariable("PayoneerId");
            });

            services.AddTransient<IAdministrationService, AdministrationService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IPartnerService, PartnerService>();
            services.AddTransient<ICreatorService, CreatorService>();
            services.AddTransient<IHitService, HitService>();
            services.AddTransient<IStripeService, StripeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPaymentLogsService, PaymentLogsService>();
            services.AddMemoryCache();
            return services;
        }
    }
}
