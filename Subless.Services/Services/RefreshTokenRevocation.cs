// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.Bff;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    /// <summary>
    /// Cookie configuration to revoke refresh token on logout.
    /// </summary>
    public class RefreshTokenRevocation : PostConfigureApplicationCookieRevokeRefreshToken, IPostConfigureOptions<CookieAuthenticationOptions>
    {
        private readonly BffOptions _options;
        private readonly IUserAccessTokenStore userAccessTokenStore;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string _scheme;
        private readonly AuthSettings authSettings;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bffOptions"></param>
        /// <param name="authOptions"></param>
        public RefreshTokenRevocation(
            BffOptions bffOptions,
            IOptions<AuthenticationOptions> authOptions,
            IOptions<AuthSettings> authSettings,
            IUserAccessTokenStore userAccessTokenStore,
            IHttpClientFactory httpClientFactory,
            ILoggerFactory logger) :
            base(bffOptions, authOptions, logger.CreateLogger<PostConfigureApplicationCookieRevokeRefreshToken>())
        {
            if (authOptions is null)
            {
                throw new ArgumentNullException(nameof(authOptions));
            }

            if (authSettings is null)
            {
                throw new ArgumentNullException(nameof(authSettings));
            }

            _options = bffOptions ?? throw new ArgumentNullException(nameof(bffOptions));
            this.userAccessTokenStore = userAccessTokenStore ?? throw new ArgumentNullException(nameof(userAccessTokenStore));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _scheme = authOptions.Value.DefaultAuthenticateScheme ?? authOptions.Value.DefaultScheme;
            this.authSettings = authSettings.Value ?? throw new ArgumentNullException(nameof(authSettings));
        }

        /// <inheritdoc />
        public new void PostConfigure(string name, CookieAuthenticationOptions options)
        {
            if (_options.RevokeRefreshTokenOnLogout && name == _scheme)
            {
                options.Events.OnSigningOut = CreateCallback(options.Events.OnSigningOut);
            }
        }

        private Func<CookieSigningOutContext, Task> CreateCallback(Func<CookieSigningOutContext, Task> inner)
        {
            async Task Callback(CookieSigningOutContext ctx)
            {
                var tokens = await userAccessTokenStore.GetTokenAsync(ctx.HttpContext.User);
                if (tokens != null && tokens.RefreshToken != null)
                {
                    var response = await httpClientFactory.CreateClient().PostAsync(authSettings.IssuerUrl + $"/oauth2/revoke?token={tokens.RefreshToken}&client_id={authSettings.AppClientId}", null);
                    await inner?.Invoke(ctx);
                }
                // await ctx.HttpContext.RevokeUserRefreshTokenAsync();
            };

            return Callback;
        }
    }
}
