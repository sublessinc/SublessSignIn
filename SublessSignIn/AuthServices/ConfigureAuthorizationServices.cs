using Duende.Bff;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SublessSignIn.AuthServices
{
    public static class ConfigureAuthorizationServices
    {
        public static IServiceCollection AddBffServices(this IServiceCollection services, AuthSettings AuthSettings)
        {
            var cookieServices = services.Where<ServiceDescriptor>(x => x.ServiceType == typeof(IPostConfigureOptions<CookieAuthenticationOptions>));
            services.AddBff().AddServerSideSessions();

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
                    options.Cookie.Name = "__Host-spa";

                    // strict SameSite handling
                    options.Cookie.SameSite = SameSiteMode.None;
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = AuthSettings.CognitoUrl;

                    // confidential client using code flow + PKCE + query response mode
                    options.ClientId = AuthSettings.AppClientId;
                    options.ResponseType = "code";
                    options.ResponseMode = "query";
                    options.UsePkce = true;
                    options.MapInboundClaims = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Events = new OpenIdConnectEvents()
                    {
                        //handle the logout redirection
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            // TODO FIX THAT URL
                            var logouturi = AuthSettings.IssuerUrl + $"/logout?response_type=code&client_id={AuthSettings.AppClientId}&logout_uri=https://localhost:4200";
                            context.Response.Redirect(logouturi);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                    // save access and refresh token to enable automatic lifetime management
                    options.SaveTokens = true;

                    // request scopes
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");

                });

            return services;
        }

    }
}
