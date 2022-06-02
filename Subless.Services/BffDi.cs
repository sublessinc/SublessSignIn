using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duende.Bff;
using Duende.Bff.EntityFramework;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Subless.Models;
using SublessSignIn.AuthServices;

namespace Subless.Services
{
    public static class BffDi
    {
        public static IServiceCollection AddBffDi(IServiceCollection services, AuthSettings AuthSettings)
        {
            var cookieServices = services.Where<ServiceDescriptor>(x => x.ServiceType == typeof(IPostConfigureOptions<CookieAuthenticationOptions>));
            services.AddBff(a => a.LicenseKey = AuthSettings.IdentityServerLicenseKey).AddEntityFrameworkServerSideSessions(options =>
            {
                options.UseNpgsql(AuthSettings.SessionStoreConnString, sqlOpts =>
                {
                    sqlOpts.MigrationsAssembly(typeof(Duende.Bff.EntityFramework.SessionDbContext).Assembly.FullName);
                });

            });

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
            return services;
        }

    }
}
