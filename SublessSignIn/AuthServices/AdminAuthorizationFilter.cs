using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Subless.Services.Services;
using System;

namespace Subless.Services
{
    public sealed class AdminAuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly IUserService _userService;
        private readonly ILogger logger;

        public IUserService userService {
            get {
                return _userService;
            }
        }

        #pragma warning disable CA1019 // The loggerfactory is not stored, so we do not need to expose it.
        public AdminAuthorizationFilter(IUserService userService, ILoggerFactory loggerFactory)
        {
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.logger = loggerFactory?.CreateLogger<AdminAuthorizationFilter>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var cognitoId = userService.GetUserClaim(context.HttpContext.User);
            if (!userService.IsUserAdmin(cognitoId))
            {
                logger.LogError($"Cognito user {cognitoId} tried to access disallowed route");
                context.Result = new ForbidResult();
            }
        }
    }
}
