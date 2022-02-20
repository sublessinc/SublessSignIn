using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            this.AuthSettings = authSettingsOptions.Value;
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
            //MS Said this only expires to stop build up of dead cookies
            options.CorrelationCookie.Expiration = TimeSpan.FromDays(7);
            options.NonceCookie.SameSite = SameSiteMode.Lax;
            //MS Said this only expires to stop build up of dead cookies
            options.NonceCookie.Expiration = TimeSpan.FromDays(7);
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

            options.Events.OnTicketReceived = async context =>
            {
                Console.WriteLine("Test");
            };
            options.Events.OnUserInformationReceived = async context =>
            {//2
                Console.WriteLine("Test");
                Console.WriteLine(context.Principal.Claims.First(x => x.Type == "cognito:username").Value);
                Console.WriteLine(context.User);
                Console.WriteLine(context.Properties.RedirectUri);
                var cognitoId = userService.GetUserClaim(context.Principal);
                if (!stripeService.CustomerHasPaid(cognitoId))
                {
                    context.Properties.RedirectUri = "/";
                }
                // if (!userservice.HasPaid(cognitoid))
                // { context.Properties.RedirectUri == "/" }

            };
            options.Events.OnTokenValidated = async context =>
            {//1
                Console.WriteLine("Test");
                Console.WriteLine(context.Principal.Claims.First(x => x.Type == "cognito:username").Value);
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