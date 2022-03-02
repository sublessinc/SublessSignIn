using Duende.Bff;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Subless.Models;
using System.Linq;

namespace SublessSignIn.AuthServices
{
    /// <summary>
    /// We're using a BFF implementation from Duende, but we have to override a few behaviors
    /// 1) Cognito doesn't support the OIDC revoke behavior, but they have an endpoint we can call manually, so we're overriding that in SublessLogoutSerivce
    /// 2) Duende BFF doesn't allow non-local redirects for login/logout, but we will explicitly be redirecting to and from 3rd parties, so we override that
    /// 3) Our SSL setup is first hop, handled at the load balancer in ECS, so we need to override some URLs to be HTTPs, because the server thinks it's running in HTTP
    /// </summary>
    public static class ConfigureAuthorizationServices
    {
        public static IServiceCollection AddBffServices(this IServiceCollection services, AuthSettings AuthSettings)
        {
            var cookieServices = services.Where<ServiceDescriptor>(x => x.ServiceType == typeof(IPostConfigureOptions<CookieAuthenticationOptions>));
            services.AddBff(a => a.LicenseKey = AuthSettings.IdentityServerLicenseKey).AddServerSideSessions();

            var descriptor =
                new ServiceDescriptor(
                    typeof(IPostConfigureOptions<CookieAuthenticationOptions>),
                    typeof(RefreshTokenRevocation),
                    ServiceLifetime.Singleton);
            services.RemoveAll<IPostConfigureOptions<CookieAuthenticationOptions>>();
            foreach (var cookieSerivce in cookieServices)
            {
                services.Add(cookieSerivce);
            }
            services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, RefreshTokenRevocation>();
            services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureApplicationValidatePrincipal>();
            services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureApplicationCookieTicketStore>();

            // configure server-side authentication and session management
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
                .AddCookie("cookie", options =>
                {
                    // host prefixed cookie name
                    options.Cookie.Name = "subless";
                    // Samesite has to be none to support hit tracking
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddOpenIdConnect("oidc", options =>
                {


                });
            services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsHandler>();

            return services;
        }

    }
}
