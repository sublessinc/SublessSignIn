﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services.Services;
using Subless.Services.Services.SublessStripe;

namespace SublessSignIn.AuthServices
{
    public class OpenIdConnectOptionsHandler : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly IStripeService stripeService;
        private readonly IUserService userService;
        private readonly AuthSettings AuthSettings;

        public OpenIdConnectOptionsHandler(IUserService userService, IStripeService stripeService, IOptions<AuthSettings> authSettingsOptions)
        {
            if (authSettingsOptions is null)
            {
                throw new ArgumentNullException(nameof(authSettingsOptions));
            }

            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            AuthSettings = authSettingsOptions.Value;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            options.Authority = AuthSettings.CognitoUrl;

            // confidential client using code flow + PKCE + query response mode
            options.ClientId = AuthSettings.AppClientId;
            options.ResponseType = "code";
            options.ResponseMode = "query";
            options.UsePkce = true;
            options.MapInboundClaims = false;

            //These need to be lax in order to handle both remote logins and the first-hop SSL configuration on the ECS cluster
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.Expiration = TimeSpan.FromHours(15);
            options.NonceCookie.SameSite = SameSiteMode.Lax;
            options.NonceCookie.Expiration = TimeSpan.FromHours(15);
            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = true;
            options.Events = new OpenIdConnectEvents()
            {
                //handle the logout redirection
                OnRedirectToIdentityProviderForSignOut = context =>
                {
                    var logouturi = AuthSettings.IssuerUrl + $"/logout?client_id={AuthSettings.AppClientId}&logout_uri={context.ProtocolMessage.RedirectUri ?? AuthSettings.Domain}";
                    context.Response.Redirect(logouturi);
                    context.HandleResponse();

                    return Task.CompletedTask;
                },
            };
            var redirectToIdpHandler = options.Events.OnRedirectToIdentityProvider;
            options.Events.OnRedirectToIdentityProvider = async context =>
            {
                // Call what Microsoft.Identity.Web is doing
                await redirectToIdpHandler(context);
                // Override the redirect URI to be what you want
                context.ProtocolMessage.RedirectUri = $"{AuthSettings.Domain}signin-oidc";
            };
            options.Events.OnUserInformationReceived = async context =>
            {
                var cognitoId = userService.GetUserClaim(context.Principal);
                if (!stripeService.CustomerHasPaid(cognitoId))
                {
                    context.Response.Cookies.Append("returnUri", context.Properties.RedirectUri, new CookieOptions()
                    {
                        MaxAge = TimeSpan.FromMinutes(15)
                    });
                    context.Properties.RedirectUri = "/";
                }
            };
            // save access and refresh token to enable automatic lifetime management
            options.SaveTokens = true;

            // request scopes
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
        }

        public void Configure(OpenIdConnectOptions options)
        {
            Configure("oidc", options);
        }
    }
}
