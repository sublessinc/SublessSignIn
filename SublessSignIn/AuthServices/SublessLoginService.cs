using System.Linq;
using System.Threading.Tasks;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace SublessSignIn
{
    public class SublessLoginService : ILoginService
    {
        private readonly BffOptions _options;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options"></param>
        public SublessLoginService(BffOptions options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public async Task ProcessRequestAsync(HttpContext context)
        {

            var returnUrl = context.Request.Query[Constants.RequestParameters.ReturnUrl].FirstOrDefault();

            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/"
            };

            await context.ChallengeAsync(props);
        }
    }
}