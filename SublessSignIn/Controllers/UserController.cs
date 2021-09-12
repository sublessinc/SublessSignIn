using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Subless.Models;
using Subless.Services;
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
        private readonly ILogger _logger;
        public UserController(IStripeService stripeService, ILoggerFactory loggerFactory, IUserService userService)
        {
            this.stripeService = stripeService;
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpDelete()]
        public void DeleteUser()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            stripeService.CancelSubscription(cognitoId);
            var user = userService.GetUserByCognitoId(cognitoId);
            userService.DemoteUser(user.Id);

            throw new NotImplementedException();
        }
    }
}
