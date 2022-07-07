using System;
using System.Linq;
using System.Threading.Tasks;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace SublessSignIn
{
    public class SublessLogoutService : ILogoutService
    {
        private readonly BffOptions _options;
        private readonly IAuthenticationSchemeProvider _schemes;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="authenticationSchemeProvider"></param>
        public SublessLogoutService(BffOptions options, IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _options = options;
            _schemes = authenticationSchemeProvider;
        }

        /// <inheritdoc />
        public async Task ProcessRequestAsync(HttpContext context)
        {
            var result = await context.AuthenticateAsync();
            if (result.Succeeded && result.Principal.Identity.IsAuthenticated)
            {
                var userSessionId = result.Principal.FindFirst(JwtClaimTypes.SessionId)?.Value;
                if (!String.IsNullOrWhiteSpace(userSessionId))
                {
                    var passedSessionId = context.Request.Query[JwtClaimTypes.SessionId].FirstOrDefault();
                    // for an authenticated user, if they have a sesison id claim,
                    // we require the logout request to pass that same value to
                    // prevent unauthenticated logout requests (similar to OIDC front channel)
                    if (_options.RequireLogoutSessionId && userSessionId != passedSessionId)
                    {
                        throw new UnauthorizedAccessException("Invalid Session Id");
                    }
                }
            }

            // get rid of local cookie first
            var signInScheme = await _schemes.GetDefaultSignInSchemeAsync();
            await context.SignOutAsync(signInScheme.Name);

            var returnUrl = context.Request.Query[Constants.RequestParameters.ReturnUrl].FirstOrDefault();

            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/"
            };

            // trigger idp logout
            await context.SignOutAsync(props);
        }
    }
}
