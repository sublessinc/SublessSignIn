using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IUsageService _usageService;
        private readonly ILogger _logger;
        public UserController(
            IStripeService stripeService,
            ILoggerFactory loggerFactory,
            IUserService userService,
            ICognitoService cognitoService,
            IPartnerService partnerService,
            IHitService hitService,
            IPaymentLogsService paymentLogsService,
            IUsageService usageService)
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
            _usageService = usageService ?? throw new ArgumentNullException(nameof(usageService));
            _logger = loggerFactory?.CreateLogger<PartnerController>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        [HttpDelete()]
        public async Task DeleteUser()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            await Delete(cognitoId);
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpDelete("{cognitoId}")]
        public async Task DeleteUser(string cognitoId)
        {
            await Delete(cognitoId);
        }

        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpDelete("byemail")]
        public async Task DeleteUserByEmail([FromQuery] string email)
        {
            var cognitoId = await cognitoService.GetCongitoUserByEmail(email);
            await Delete(cognitoId);
        }

        private async Task Delete(string cognitoId)
        {
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
            var paymentPeriod = paymentLogsService.GetLastPaymentPeriod();
            _usageService.SaveUsage(UsageType.UserStats, user.Id);

            if (paymentPeriod == null)
            {

                // DEPRECATED

                var paymentDate = paymentLogsService.GetLastPaymentDate();
                if (paymentDate == DateTimeOffset.MinValue)
                {
                    paymentDate = DateTimeOffset.UtcNow.AddMonths(-1);
                }

                // END DEPRECATED
                return Ok(new HistoricalStats<UserStats>
                {
                    LastMonth = hitService.GetUserStats(paymentDate.AddMonths(-1), paymentDate, user.Id),
                    thisMonth = hitService.GetUserStats(paymentDate, DateTimeOffset.UtcNow, user.Id)
                });
            }
            else
            {
                return Ok(new HistoricalStats<UserStats>
                {
                    LastMonth = hitService.GetUserStats(paymentPeriod.Item1, paymentPeriod.Item2, user.Id),
                    thisMonth = hitService.GetUserStats(paymentPeriod.Item2, DateTimeOffset.UtcNow, user.Id)
                });
            }

        }

        [HttpGet("loggedIn")]
        [EnableCors("Unrestricted")]
        [AllowAnonymous]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public ActionResult<bool> GetLoggedIn()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Ok(false);
            }
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            var user = userService.GetUserByCognitoId(cognitoId);
            if (user != null)
            {
                _usageService.SaveUsage(UsageType.Visit, user.Id);
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet("loginStatus")]
        [EnableCors("Unrestricted")]
        [AllowAnonymous]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public ActionResult<LoggedInEnum> GetLoggedInRenewal()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Ok(LoggedInEnum.NotLoggedIn);
            }
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            var user = userService.GetUserByCognitoId(cognitoId);
            if (user == null)
            {
                return Ok(LoggedInEnum.NotLoggedIn);
            }
            if (HttpContext.Items.TryGetValue("ExpiresUTC", out var expirationDate)
                && expirationDate is DateTimeOffset
                && (DateTimeOffset)expirationDate < DateTime.UtcNow.AddDays(3))
            {
                return Ok(LoggedInEnum.ShouldRenew);
            }
            return Ok(LoggedInEnum.LoggedIn);
        }

        [HttpGet("RecentFeed")]
        public ActionResult<IEnumerable<HitView>> RecentFeed()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                return Ok(hitService.GetRecentPatronContent(cognitoId));
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");

                return Unauthorized();
            }
        }

        [HttpGet("TopFeed")]
        public ActionResult<IEnumerable<CreatorHitCount>> TopFeed()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            try
            {
                return Ok(hitService.GetTopPatronContent(cognitoId).Select(x=> new CreatorHitCount { CreatorName= x.CreatorName, Hits= x.Hits}));
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogWarning(e, "Unauthorized user attempted to get creator stats");
                return Unauthorized();
            }
        }

        [HttpPut("terms")]
        public ActionResult AcceptTerms()
        {
            var cognitoId = userService.GetUserClaim(HttpContext.User);
            userService.AcceptTerms(cognitoId);
            return Ok();
        }
    }
}
