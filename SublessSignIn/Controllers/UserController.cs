﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Subless.Models;
using Subless.Services;
using Subless.Services.Services;
using SublessSignIn.Models;


namespace SublessSignIn.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IStripeService stripeService;
        private readonly IUserService userService;
        private readonly ICognitoService cognitoService;
        private readonly ILogger _logger;
        public UserController(IStripeService stripeService, ILoggerFactory loggerFactory, IUserService userService, ICognitoService cognitoService)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this.stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.cognitoService = cognitoService ?? throw new ArgumentNullException(nameof(cognitoService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpDelete()]
        public async Task DeleteUser()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            stripeService.CancelSubscription(cognitoId);
            var user = userService.GetUserByCognitoId(cognitoId);
            userService.DemoteUser(user.Id);
            await cognitoService.DeleteCognitoUser(user.CognitoId);
        }
    }
}