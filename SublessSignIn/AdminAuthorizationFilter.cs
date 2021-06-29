﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Subless.Services
{
    public class AdminAuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly IUserService userService;
        private readonly ILogger logger;

        public AdminAuthorizationFilter(IUserService userService, ILoggerFactory loggerFactory)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.logger = loggerFactory?.CreateLogger<AdminAuthorizationFilter>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var cognitoId = context.HttpContext.User.Claims.First(x => x.Type == "cognito:username")?.Value;
            if (!userService.IsUserAdmin(cognitoId))
            {
                logger.LogError($"Cognito user {cognitoId} tried to access disallowed route");
                context.Result = new ForbidResult();
            }
        }
    }
}
