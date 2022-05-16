using Microsoft.Extensions.DependencyInjection;
using Subless.Models;
using Subless.Services.Services;
using System;

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
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? throw new ArgumentNullException("DOMAIN");
                if (!options.Domain.EndsWith('/'))
                {
                    options.Domain += '/';
                }
                options.SublessPayPalId = Environment.GetEnvironmentVariable("PayPalId");
            });
            services.Configure<DomainConfig>(options =>
            {
                options.Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? throw new ArgumentNullException("DOMAIN");
                options.Region = Environment.GetEnvironmentVariable("region") ?? throw new ArgumentNullException("region");
                options.UserPool = Environment.GetEnvironmentVariable("userPoolId") ?? throw new ArgumentNullException("userPoolId");
            });
            services.Configure<FeatureConfig>(options =>
            {
                options.HitPopupEnabled = bool.TryParse(Environment.GetEnvironmentVariable("HitPopupEnabled"), out bool popupEnabled) ? popupEnabled : false;
            });

            services.AddTransient<IAdministrationService, AdministrationService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IPartnerService, PartnerService>();
            services.AddTransient<ICreatorService, CreatorService>();
            services.AddTransient<IHitService, HitService>();
            services.AddTransient<IStripeService, StripeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPaymentLogsService, PaymentLogsService>();
            services.AddTransient<ICognitoService, CognitoService>();
            services.AddTransient<ICacheService, CacheService>();
            services.AddTransient<IUsageService, UsageService>();
            services.AddMemoryCache();
            services.AddHttpClient();
            return services;
        }
    }
}
