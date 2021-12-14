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
        private readonly BffOptions _options;
        private readonly AuthSettings authSettings;
        private readonly ILogger<SublessLoginService> _logger;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options"></param>
        public SublessLoginService(BffOptions options, ILoggerFactory loggerFactory, IOptions<AuthSettings> authSettings)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (authSettings is null || authSettings.Value is null)
            {
                throw new ArgumentNullException(nameof(authSettings));
            }

            _logger = loggerFactory.CreateLogger<SublessLoginService>();
            _options = options ?? throw new ArgumentNullException(nameof(options));
            this.authSettings = authSettings.Value;
        }

        /// <inheritdoc />
        public async Task ProcessRequestAsync(HttpContext context)
        {

            var returnUrl = context.Request.Query[Constants.RequestParameters.ReturnUrl].FirstOrDefault();
            //_logger.LogInformation($"ReturnUrl: {returnUrl}");
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/",
            };
            var publicDomain = new Uri(authSettings.Domain);
            //_logger.LogInformation($"Transformed ReturnUri {props.RedirectUri}");
            //context.Request.Host =  new HostString(publicDomain.Host, publicDomain.Port);
            //context.Request.Scheme = publicDomain.Scheme;
            await context.ChallengeAsync(props);
        }
    }
}
