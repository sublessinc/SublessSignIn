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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly IOptions<StripeConfig> _stripeConfig;
        private readonly IUserService _userService;
        private readonly IStripeService _stripeService;
        private readonly ILogger<CheckoutController> _logger;
        public CheckoutController(ILoggerFactory loggerFactory, IOptions<StripeConfig> stripeConfig, IUserService userService, IStripeService stripeService)
        {
            _stripeConfig = stripeConfig ?? throw new ArgumentNullException(nameof(stripeConfig));
            _ = stripeConfig.Value.PublishableKey ?? throw new ArgumentNullException(nameof(stripeConfig.Value.PublishableKey));
            _ = stripeConfig.Value.BasicPrice ?? throw new ArgumentNullException(nameof(stripeConfig.Value.BasicPrice));
            _ = stripeConfig.Value.Domain ?? throw new ArgumentNullException(nameof(stripeConfig.Value.Domain));
            _ = stripeConfig.Value.SecretKey ?? throw new ArgumentNullException(nameof(stripeConfig.Value.SecretKey));
            _ = stripeConfig.Value.WebhookSecret ?? throw new ArgumentNullException(nameof(stripeConfig.Value.WebhookSecret));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            _logger = loggerFactory.CreateLogger<CheckoutController>();
        }

        /// <summary>
        /// Provides environment data to stripe front end
        /// </summary>
        [HttpGet("setup")]
        public StripeCheckoutViewModel Setup()
        {
            return new StripeCheckoutViewModel
            {
                BasicPrice = _stripeConfig.Value.BasicPrice,
                PublishableKey = _stripeConfig.Value.PublishableKey,
            };

        }

        /// <summary>
        /// Asks stripe server to create a new transaction, 
        /// and then provides the frontend with a link to that transaction checkout
        /// </summary>
        /// 
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
        {
            var cognitoId = User.FindFirst("username")?.Value;
            if (cognitoId == null)
            {
                return Unauthorized();
            }

            try
            {

                return Ok(await _stripeService.CreateCheckoutSession(req.PriceId, cognitoId));
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Could not create stripe session " + e.StripeError.Message);
                return BadRequest();
            }
        }

        [HttpPost("customer-portal")]
        public async Task<IActionResult> CustomerPortal()
        {
            var cognitoId = User.FindFirst("username")?.Value;
            if (cognitoId == null)
            {
                return Unauthorized();
            }

            return Ok(await _stripeService.GetCustomerPortalLink(cognitoId));
        }

        /// <summary>
        /// Retreives the session information from stripe after the user completed the payment
        /// </summary>
        [HttpGet("checkout-session")]
        public async Task<IActionResult> CheckoutSession(string sessionId)
        {
            var cognitoId = User.FindFirst("username")?.Value;
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            return Ok(await _stripeService.GetSession(sessionId));
        }

        [HttpGet("existing-session")]
        public IActionResult GetUserSession()
        {
            var cognitoId = User.FindFirst("username")?.Value;
            if (cognitoId == null)
            {
                return Unauthorized();
            }
            return Ok(new SessionId()
            {
                Id = _userService.GetStripeIdFromCognitoId(cognitoId)
            });
        }
        private class SessionId
        {
            public string Id { get; set; }
        }
    }
}
