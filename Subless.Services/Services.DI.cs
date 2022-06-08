using Microsoft.Extensions.DependencyInjection;
using Subless.Models;
using Subless.Services.Extensions;
using Subless.Services.Services;
using System;

namespace Subless.Services
{
    public static class ServicesDi
    {
        public static IServiceCollection AddServicesDi(IServiceCollection services)
        {

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
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICalculatorService, CalculatorService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IFileStorageService, S3Service>();
            services.AddTransient<AwsCredWrapper, AwsCredWrapper>();
            services.AddTransient<IPaymentEmailService, PaymentEmailService>();
            services.AddMemoryCache();
            services.AddHttpClient();
            return services;
        }
    }
}
