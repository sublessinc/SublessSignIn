using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Subless.Models;
using Subless.Services;
using Subless.Services.Extensions;
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
        private readonly IPartnerService partnerService;
        private readonly IHitService hitService;
        private readonly IPaymentLogsService paymentLogsService;
        private readonly ILogger _logger;
        public UserController(
            IStripeService stripeService, 
            ILoggerFactory loggerFactory, 
            IUserService userService, 
            ICognitoService cognitoService, 
            IPartnerService partnerService, 
            IHitService hitService,
            IPaymentLogsService paymentLogsService)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this.stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.cognitoService = cognitoService ?? throw new ArgumentNullException(nameof(cognitoService));
            this.partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            this.hitService = hitService ?? throw new ArgumentNullException(nameof(hitService));
            this.paymentLogsService = paymentLogsService ?? throw new ArgumentNullException(nameof(paymentLogsService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpDelete()]
        public async Task DeleteUser()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            stripeService.CancelSubscription(cognitoId);
            var user = userService.GetUserByCognitoId(cognitoId);
            if (user.Creators.Any())
            {
                foreach (var creator in user.Creators)
                {
                    await partnerService.CreatorChangeWebhook(creator.ToPartnerView());
                }
            }
            userService.DemoteUser(user.Id);
            await cognitoService.DeleteCognitoUser(user.CognitoId);
        }

        [HttpGet()]
        public ActionResult<UserViewModel> GetUser()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            var user = userService.GetUserByCognitoId(cognitoId);
            var viewModel = user.ToViewModel(HttpContext.User.FindFirst("email").Value);
            return Ok(viewModel);
        }

        [HttpGet("Analytics")]
        public ActionResult<UserStats> GetUserAnalytics()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            var user = userService.GetUserByCognitoId(cognitoId);
            var paymentDate = paymentLogsService.GetLastPaymentDate();
            if (paymentDate == DateTimeOffset.MinValue)
            {
                paymentDate = DateTimeOffset.UtcNow.AddMonths(-1);
            }
            var hitsThisMonth = hitService.GetHitsByDate(paymentDate, DateTimeOffset.UtcNow, user.Id);
            var hitsLastMonth = hitService.GetHitsByDate(paymentDate.AddMonths(-1), paymentDate, user.Id);
            return Ok(UserStatsExtensions.GetHistoricalUserStats(hitsThisMonth, hitsLastMonth));
        }

        [HttpGet("loggedIn")]
        [EnableCors("Unrestricted")]
        [AllowAnonymous]
        public ActionResult<bool> GetLoggedIn()
        {
            if (!this.HttpContext.User.Identity.IsAuthenticated)
            {
                return Ok(false);
            }
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            var user = userService.GetUserByCognitoId(cognitoId);
            if (user!=null)
            {
                return Ok(true);
            }
            return Ok(false);
        }

    }
}
