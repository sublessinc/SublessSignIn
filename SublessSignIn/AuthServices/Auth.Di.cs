﻿using Duende.Bff;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Subless.Models;

namespace SublessSignIn.AuthServices
{
    public static class AuthDi
    {
        public static IServiceCollection RegisterAuthDi(this IServiceCollection services, AuthSettings authSettings)
        {
            services.Configure<AuthSettings>(options =>
            {
                options.AppClientId = authSettings.AppClientId;
                options.CognitoUrl = authSettings.CognitoUrl;
                options.IssuerUrl = authSettings.IssuerUrl;
                options.JwtKeySetUrl = authSettings.JwtKeySetUrl;
                options.PoolId = authSettings.PoolId;
                options.Region = authSettings.Region;
                options.Domain = authSettings.Domain;
                options.IdentityServerLicenseKey = authSettings.IdentityServerLicenseKey;
                options.SessionStoreConnString = authSettings.SessionStoreConnString;
            });

            services.AddTransient<ILoginService, SublessLoginService>();
            services.AddTransient<ILogoutService, SublessLogoutService>();
            services.AddTransient<ICorsPolicyAccessor, CorsPolicyAccessor>();
            services.AddTransient<OpenIdConnectHandler, SublessOpenIdConnectHandler>();
            return services;
        }
    }
}
