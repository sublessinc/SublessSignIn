using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Subless.Data;
using Subless.Models;
using Subless.Services;
using static Subless.Data.DataDi;

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
            services = BffDi.AddBffDi(services, AuthSettings);
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

                    options.Cookie.MaxAge = TimeSpan.FromDays(14);
                    options.Events.OnValidatePrincipal = AddTokenExpirationData;
                })
                .AddOpenIdConnect("oidc", options =>
                {

                });
            services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsHandler>();

            //Add data protection to ensure both services can unencrypt response messages

            var json = Environment.GetEnvironmentVariable("dbCreds");
            var dbCreds = JsonConvert.DeserializeObject<DbCreds>(json);

            // Add a DbContext to store your Database Keys
            services.AddDbContext<KeyStorageContext>(options =>
                options.UseNpgsql(
                    dbCreds.GetDatabaseConnection()));

            // using Microsoft.AspNetCore.DataProtection;
            services.AddDataProtection()
                .PersistKeysToDbContext<KeyStorageContext>();

            return services;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CA1822 // Mark members as static
        private static async Task AddTokenExpirationData(CookieValidatePrincipalContext context)
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            context.Request.HttpContext.Items.Add("ExpiresUTC", context.Properties.ExpiresUtc);
        }

    }
}
