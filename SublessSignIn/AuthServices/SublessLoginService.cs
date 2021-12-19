using System;
using System.Linq;
using System.Threading.Tasks;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace SublessSignIn
{
    public class SublessLoginService : ILoginService
    { 
        /// <inheritdoc />
        public async Task ProcessRequestAsync(HttpContext context)
        {

            var returnUrl = context.Request.Query[Constants.RequestParameters.ReturnUrl].FirstOrDefault();
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/",
            };
            await context.ChallengeAsync(props);
        }
    }
}
