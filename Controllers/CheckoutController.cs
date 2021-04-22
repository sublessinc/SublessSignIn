using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using SublessSignIn.Models;

namespace SublessSignIn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IOptions<StripeConfig> _stripeConfig;
        private readonly IStripeClient _client;

        public CheckoutController(IOptions<StripeConfig> stripeConfig)
        {            
            _stripeConfig = stripeConfig ?? throw new ArgumentNullException(nameof(stripeConfig));
            _ = stripeConfig.Value.PublishableKey ?? throw new ArgumentNullException(nameof(stripeConfig.Value.PublishableKey));
            _ = stripeConfig.Value.BasicPrice ?? throw new ArgumentNullException(nameof(stripeConfig.Value.BasicPrice));
            _ = stripeConfig.Value.Domain ?? throw new ArgumentNullException(nameof(stripeConfig.Value.Domain));
            _ = stripeConfig.Value.SecretKey ?? throw new ArgumentNullException(nameof(stripeConfig.Value.SecretKey));
            _ = stripeConfig.Value.WebhookSecret ?? throw new ArgumentNullException(nameof(stripeConfig.Value.WebhookSecret));
            _client = new StripeClient(_stripeConfig.Value.SecretKey);

        }
        [HttpGet("setup")]
        public StripeCheckoutViewModel Setup()
        {
            return new StripeCheckoutViewModel
            {
                BasicPrice = _stripeConfig.Value.BasicPrice,
                PublishableKey = _stripeConfig.Value.PublishableKey,
            };

        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{_stripeConfig.Value.Domain}/success.html?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_stripeConfig.Value.Domain}/canceled.html",
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

}
