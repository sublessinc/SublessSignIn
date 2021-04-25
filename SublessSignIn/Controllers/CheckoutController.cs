using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Subless.Data;
using SublessSignIn.Models;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IOptions<StripeConfig> _stripeConfig;
        private readonly IStripeClient _client;
        private readonly IUserRepository _userRepository;

        public CheckoutController(IOptions<StripeConfig> stripeConfig, IUserRepository userRepository)
        {
            _stripeConfig = stripeConfig ?? throw new ArgumentNullException(nameof(stripeConfig));
            _ = stripeConfig.Value.PublishableKey ?? throw new ArgumentNullException(nameof(stripeConfig.Value.PublishableKey));
            _ = stripeConfig.Value.BasicPrice ?? throw new ArgumentNullException(nameof(stripeConfig.Value.BasicPrice));
            _ = stripeConfig.Value.Domain ?? throw new ArgumentNullException(nameof(stripeConfig.Value.Domain));
            _ = stripeConfig.Value.SecretKey ?? throw new ArgumentNullException(nameof(stripeConfig.Value.SecretKey));
            _ = stripeConfig.Value.WebhookSecret ?? throw new ArgumentNullException(nameof(stripeConfig.Value.WebhookSecret));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _client = new StripeClient(_stripeConfig.Value.SecretKey);

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
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{_stripeConfig.Value.Domain}/PayingCustomer.html?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_stripeConfig.Value.Domain}/LoggedInButNotPaid.html",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = req.PriceId,
                        Quantity = 1,
                    },
                },
            };

            var service = new SessionService(_client);
            try
            {
                var session = await service.CreateAsync(options);
                _userRepository.AddUser(new User { StripeId = session.Id });
                //TODO: Save session ID, this will be how we manage billing later
                return Ok(new CreateCheckoutSessionResponse
                {
                    SessionId = session.Id,
                });
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest();
            }
        }

        [HttpPost("customer-portal")]
        public async Task<IActionResult> CustomerPortal()
        {
            // TODO: Switch this to loading the session ID based on the cognito user id
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID. 
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutSessionId = _userRepository.Users.First().StripeId; // <-- this is fake as hell fix this when we have an IDP
            var checkoutService = new SessionService(_client);
            var checkoutSession = await checkoutService.GetAsync( checkoutSessionId);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = _stripeConfig.Value.Domain;

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = $"{returnUrl}/PayingCustomer.html?sessionId?{_userRepository.Users.First().StripeId}",
            };
            var service = new Stripe.BillingPortal.SessionService(_client);
            var session = await service.CreateAsync(options);

            return Ok(session);
        }

        /// <summary>
        /// Retreives the session information from stripe after the user completed the payment
        /// </summary>
        [HttpGet("checkout-session")]
        public async Task<IActionResult> CheckoutSession(string sessionId)
        {
            var service = new SessionService(_client);
            var session = await service.GetAsync(sessionId);
            return Ok(session);
        }
    }
}
