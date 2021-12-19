using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SublessSignIn.AuthServices
{
    /// <summary>
    /// The sites that are allowed to direct requests to us should match our list of partners.
    /// This means that when our list of partners chagnes, we also need to change the list of allowed CORS origins
    /// </summary>
    public class CorsPolicyAccessor : ICorsPolicyAccessor
    {
        public const string UnrestrictedPolicy = "Unrestricted";
        private readonly CorsOptions _options;
        private readonly IPartnerService partnerService;
        private readonly ILogger<CorsPolicyAccessor> logger;


        public CorsPolicyAccessor(IOptions<CorsOptions> options, IPartnerService partnerService, ILoggerFactory loggerFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (partnerService is null)
            {
                throw new ArgumentNullException(nameof(partnerService));
            }

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _options = options.Value;
            this.partnerService = partnerService;
            logger = loggerFactory.CreateLogger<CorsPolicyAccessor>();
        }
        public CorsPolicy GetPolicy()
        {
            return _options.GetPolicy(_options.DefaultPolicyName);
        }

        public CorsPolicy GetPolicy(string name)
        {
            return _options.GetPolicy(name);
        }

        public void SetUnrestrictedOrigins()
        {
            logger.LogInformation("Clearing and regenerating whitelists");

            var origins = partnerService.GetParterUris();
            var policy = GetPolicy(UnrestrictedPolicy);
            policy.Origins.Clear();
            foreach (var origin in origins)
            {
                GetPolicy(UnrestrictedPolicy).Origins.Add(origin.Trim('/'));

            }
        }
    }
}
