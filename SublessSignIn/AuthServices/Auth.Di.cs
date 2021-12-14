using Duende.Bff;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Subless.Models;

namespace SublessSignIn.AuthServices
{
    public static class Auth
    {
        public static IServiceCollection RegisterAuthDi(this IServiceCollection services, AuthSettings authSettings)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.Configure<AuthSettings>(options =>
            {
                options.AppClientId = authSettings.AppClientId;
                options.CognitoUrl = authSettings.CognitoUrl;
                options.IssuerUrl = authSettings.IssuerUrl;
                options.JwtKeySetUrl = authSettings.JwtKeySetUrl;
                options.PoolId = authSettings.PoolId;
                options.Region = authSettings.Region;
                options.Domain = authSettings.Domain;
            });

            services.AddTransient<ILoginService, SublessLoginService>();
            services.AddTransient<ILogoutService, SublessLogoutService>();
            services.AddTransient<ICorsPolicyAccessor, CorsPolicyAccessor>();
            return services;
        }
    }
}
