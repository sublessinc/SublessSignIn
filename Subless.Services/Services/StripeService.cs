using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Subless.Models;

namespace Subless.Services
{
    public class StripeService : IStripeService
    {
        private readonly IStripeClient _client;
        private readonly IOptions<StripeConfig> _stripeConfig;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public StripeService(IOptions<StripeConfig> stripeConfig, IUserService userService, ILoggerFactory loggerFactory)
        {
            _stripeConfig = stripeConfig ?? throw new ArgumentNullException(nameof(stripeConfig));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = loggerFactory?.CreateLogger<StripeService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _client = new StripeClient(_stripeConfig.Value.SecretKey ?? throw new ArgumentNullException(nameof(_stripeConfig.Value.SecretKey)));
        }
        public async Task<CreateCheckoutSessionResponse> CreateCheckoutSession(string priceId, string cognitoId)
        {
            var user = _userService.GetUserByCognitoId(cognitoId);
            var customer = user.StripeCustomerId;

            if (string.IsNullOrEmpty(user.StripeCustomerId))
            {
                customer = CreateCustomer(cognitoId).Id;
            }

            var options = new SessionCreateOptions
            {
                Customer = customer,
                SuccessUrl = $"{_stripeConfig.Value.Domain}/user-profile",
                CancelUrl = $"{_stripeConfig.Value.Domain}/register-payment",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                },
            };

            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);
            _userService.AddStripeSessionId(cognitoId, session.Id);
            return new CreateCheckoutSessionResponse
            {
                SessionId = session.Id,
            };
        }

        public Customer CreateCustomer(string cognitoId)
        {
            var customerDetails = new CustomerCreateOptions
            {
                Description = cognitoId
            };
            var service = new CustomerService(_client);
            var customer = service.Create(customerDetails);
            _userService.AddStripeCustomerId(cognitoId, customer.Id);
            return customer;
        }

        public async Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId)
        {
            // TODO: Switch this to loading the session ID based on the cognito user id
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID. 
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutSessionId = _userService.GetStripeIdFromCognitoId(cognitoId);
            var checkoutService = new SessionService(_client);
            var checkoutSession = await checkoutService.GetAsync(checkoutSessionId);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = _stripeConfig.Value.Domain;

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = $"{returnUrl}/user-profile",
            };
            var service = new Stripe.BillingPortal.SessionService(_client);
            return await service.CreateAsync(options);
        }

        public async Task<Session> GetSession(string sessionId)
        {
            var service = new SessionService(_client);
            return await service.GetAsync(sessionId);
        }

        public IEnumerable<Payer> GetInvoicesForRange(DateTime startDate, DateTime endDate)
        {
            var filters = new InvoiceListOptions()
            {
                Status = "paid",
                Created = new DateRangeOptions
                {
                    GreaterThan = startDate,
                    LessThanOrEqual = endDate
                }
            };
            var invoiceService = new InvoiceService(_client);
            var invoices = invoiceService.List(filters);
            var cusomterIds = invoices.Select(invoice => invoice.CustomerId);
            var users = _userService.GetUsersFromStripeIds(cusomterIds);
            var payers = new List<Payer>();
            foreach (var invoice in invoices)
            {
                var user = users.FirstOrDefault(x => x.StripeCustomerId == invoice.CustomerId);
                if (user == null)
                {
                    _logger.LogCritical($"User payment detected without corresponding user in subless system. CustomerId: {invoice.CustomerId} Email: {invoice.CustomerEmail}");
                }
                else
                {
                    payers.Add(new Payer
                    {
                        UserId = user.Id,
                        Payment = invoice.AmountPaid
                    });
                }
            }
            return payers;
        }
    }
}
