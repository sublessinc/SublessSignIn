using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Profiling;
using Stripe;
using Stripe.Checkout;
using Subless.Models;

namespace Subless.Services.Services
{
    public class StripeService : IStripeService
    {
        private readonly IOptions<StripeConfig> _stripeConfig;
        private readonly IUserService _userService;
        private readonly IStripeApiWrapperService _stripeApiWrapperService;
        private readonly ILogger _logger;

        public StripeService(IOptions<StripeConfig> stripeConfig, IUserService userService, IStripeApiWrapperService stripeApiWrapperService, ILoggerFactory loggerFactory)
        {
            _stripeConfig = stripeConfig ?? throw new ArgumentNullException(nameof(stripeConfig));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _stripeApiWrapperService = stripeApiWrapperService ?? throw new ArgumentNullException(nameof(stripeApiWrapperService));
            _logger = loggerFactory?.CreateLogger<StripeService>() ?? throw new ArgumentNullException(nameof(loggerFactory));

        }

        public async Task<bool> CanAccessStripe()
        {
            var list = _stripeApiWrapperService.CustomerService.List(new CustomerListOptions()
            {
                Limit = 1
            });
            if (list != null)
            {
                return true;
            }
            return false;
        }

        public async Task<CreateCheckoutSessionResponse> CreateCheckoutSession(long userBudget, string cognitoId)
        {
            var priceId = GetPriceIDByDollarAmount(userBudget);
            if (priceId == null)
            {
                _logger.LogCritical($"Price not found for budget of {userBudget}.");
                throw new ArgumentException("Invalid Price given.");
            }

            var user = _userService.GetUserByCognitoId(cognitoId);
            var customer = user.StripeCustomerId;

            if (!CustomerHasPaid(cognitoId))
            {
                return await NewSubscription(user, priceId);
            }
            UpgradeCustomer(customer, cognitoId, priceId);
            return null;

        }

        private void UpgradeCustomer(string customer, string cognitoId, string priceId)
        {
            var subs = GetSubscriptions(customer);
            var subscription = subs.Single();

            var items = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions {
                    Id = subscription.Items.Data[0].Id,
                    Price = priceId,
                },
            };

            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                ProrationBehavior = "create_prorations",
                Items = items,
            };
            _stripeApiWrapperService.SubscriptionService.Update(subscription.Id, options);
        }

        private async Task<CreateCheckoutSessionResponse> NewSubscription(User user, string priceId)
        {
            if (user.StripeCustomerId == null)
            {
                user.StripeCustomerId = CreateCustomer(user.CognitoId).Id;
            }
            var options = new SessionCreateOptions
            {
                Customer = user.StripeCustomerId,
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
            options.AddExtraParam("allow_promotion_codes", "true");
            var session = await _stripeApiWrapperService.SessionService.CreateAsync(options);
            _userService.AddStripeSessionId(user.CognitoId, session.Id);
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
            var customer = _stripeApiWrapperService.CustomerService.Create(customerDetails);
            _userService.AddStripeCustomerId(cognitoId, customer.Id);
            return customer;
        }

        private StripeList<Price> GetPrices()
        {
            //TODO: productoptions should filter to only susbcription plans
            var productOptions = new PriceListOptions();
            var prices = _stripeApiWrapperService.PriceService.List(productOptions);
            return prices;
        }

        private string GetPriceIDByDollarAmount(long dollarAmount)
        {
            var prices = GetPrices().ToList();
            //Stripe keeps the price in cents.
            var dollarAmountInCents = dollarAmount * 100;
            var price = prices.Where(x => x.UnitAmount == dollarAmountInCents).Single();

            return price?.Id;
        }

        public void RolloverPaymentForIdleCustomer(string customerId)
        {
            _logger.LogInformation($"Rolling over payment for idle customer {customerId}");
            var activeSubs = GetSubscriptions(customerId);
            if (!activeSubs.Any())
            {
                _logger.LogInformation("Customer {custId} cancelled their sub before we could rollover their payment", customerId);
                return;
            }
            var sub = activeSubs.First(); //TODO figure out which sub to choose
            var coupon = CreateOneTimeCoupon();
            ApplyCouponToSubscription(coupon, sub);
        }

        private Coupon CreateOneTimeCoupon()
        {
            var options = new CouponCreateOptions
            {
                Duration = "once",
                Id = "rollover" + Guid.NewGuid(),
                PercentOff = 100,
                MaxRedemptions = 1
            };
            return _stripeApiWrapperService.CouponService.Create(options);

        }

        private Subscription ApplyCouponToSubscription(Coupon coupon, Subscription sub)
        {
            var updateOptions = new SubscriptionUpdateOptions()
            {
                Coupon = coupon.Id,
            };
            return _stripeApiWrapperService.SubscriptionService.Update(sub.Id, updateOptions);
        }

        public bool CustomerHasPaid(string cognitoId)
        {

            var activePrices = GetActiveSubscriptionPriceId(cognitoId);
            var allPrices = GetPrices();
            return allPrices.Any(x => activePrices.Contains(x.Id));
        }
        private List<string> GetActiveSubscriptionPriceId(string cognitoId)
        {
            return GetActiveSubscriptionPrice(cognitoId).Select(x => x.Id).ToList();
        }

        public List<Price> GetActiveSubscriptionPrice(string cognitoId)
        {
            var prices = new List<Price>();
            var user = _userService.GetUserByCognitoId(cognitoId);
            if (user?.StripeCustomerId == null)
            {
                return prices;
            }

            var subscriptions = GetSubscriptions(user.StripeCustomerId);

            foreach (var sub in subscriptions)
            {
                if (sub.Status == "active")
                {
                    foreach (var item in sub.Items)
                    {
                        prices.Add(item.Price);
                    }
                }
            }

            return prices;
        }

        private StripeList<Subscription> GetSubscriptions(string stripeCustomerId)
        {
            var customer = _stripeApiWrapperService.CustomerService.Get(stripeCustomerId);
            var subscriptions = _stripeApiWrapperService.SubscriptionService.List(new SubscriptionListOptions()
            {
                Customer = customer.Id
            });
            return subscriptions;
        }

        public async Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId)
        {
            // TODO: Switch this to loading the session ID based on the cognito user id
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID. 
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutSessionId = _userService.GetStripeIdFromCognitoId(cognitoId);
            var checkoutSession = await _stripeApiWrapperService.SessionService.GetAsync(checkoutSessionId);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = _stripeConfig.Value.Domain;

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = $"{returnUrl}/user-profile",
            };
            return await _stripeApiWrapperService.BillingSessionService.CreateAsync(options);
        }

        public async Task<Session> GetSession(string sessionId)
        {
            return await _stripeApiWrapperService.SessionService.GetAsync(sessionId);
        }

        public IEnumerable<Payer> GetPayersForRange(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var utcutcStartDate = new DateTime(startDate.ToUniversalTime().Ticks, DateTimeKind.Utc);
            var utcutcEndDate = new DateTime(endDate.ToUniversalTime().Ticks, DateTimeKind.Utc);
            _logger.LogDebug($"Looking for invoices in time range UTC kind, UTC Time: {utcutcStartDate}  --  {utcutcEndDate}");
            //Stripe seems to convert datetimes to json and back, and use the local time when doing so. We need to pass a local time to prevent a double UTC conversion.
            List<Invoice> invoices;
            using (MiniProfiler.Current.Step("Get invoices"))
            {
                invoices = GetInvoiceInRage(utcutcStartDate, utcutcEndDate);

            }
            var cusomterIds = invoices.Select(invoice => invoice.CustomerId);
            var users = _userService.GetUsersFromStripeIds(cusomterIds);
            var payers = new ConcurrentBag<Payer>();
            using (MiniProfiler.Current.Step("Per-payer data retrieval"))
            {
                Parallel.ForEach(invoices, invoice =>
                {
                    var payer = ProcessPayer(invoice, users);
                    if (payer != null)
                    {
                        payers.Add(payer);
                    }
                });
            }
            return payers.ToList();
        }

        private Payer ProcessPayer(Invoice invoice, IEnumerable<User> users)
        {
            _logger.LogDebug($"Invoice {invoice.Id} found for date {invoice.Created}");
            var user = users.FirstOrDefault(x => x.StripeCustomerId == invoice.CustomerId);
            if (user == null)
            {
                _logger.LogWarning($"User payment detected without corresponding user in subless system. CustomerId: {invoice.CustomerId} Email: {invoice.CustomerEmail}");
            }
            else
            {
                long payment = 0;
                long fees = 0;
                Charge charge;
                BalanceTransaction balanceTrans;
                StripeList<Refund> refunds;
                var taxes = invoice?.Tax ?? 0;
                if (invoice.ChargeId != null) // Charges will not be present if the payment was made with a coupon
                {
                    using (MiniProfiler.Current.Step("Get Charge"))
                    {
                        charge = _stripeApiWrapperService.ChargeService.Get(invoice.ChargeId);

                    }
                    using (MiniProfiler.Current.Step("Get balance trans"))
                    {
                        balanceTrans = _stripeApiWrapperService.BalanceTransactionService.Get(charge.BalanceTransactionId);
                    }
                    fees = balanceTrans.Fee;
                    payment = balanceTrans.Net;
                    using (MiniProfiler.Current.Step("Get refunds"))
                    {
                        refunds = _stripeApiWrapperService.RefundService.List(new RefundListOptions() { Charge = invoice.ChargeId });
                    }
                    // Check to see if it was a full refund. Set payment to 0 if it was.
                    if (refunds.Any())
                    {
                        var totalRefund = refunds.Select(x => x.Amount).Sum();
                        payment = balanceTrans.Amount - totalRefund;
                        // Fees are subtracted from the payment by stripe. If we refunded less than the payment, we have to manually address the fees
                        payment = payment - fees;
                        if (payment < 0)
                        {
                            payment = 0;
                        }
                    }
                }
                // if we were paid with a coupon, we need to calculate the payment differently
                else
                {
                    payment = invoice.Subtotal;
                }

                return new Payer
                {
                    UserId = users.Single(x => x.StripeCustomerId == invoice.CustomerId).Id,
                    Payment = payment,
                    Taxes = taxes,
                    Fees = fees
                };
            }
            return null;
        }

        private List<Invoice> GetInvoiceInRage(DateTime startDate, DateTime endDate)
        {
            var invoices = new List<Invoice>();
            var filters = new InvoiceListOptions()
            {
                Status = "paid",
                Created = new DateRangeOptions
                {
                    GreaterThan = startDate,
                    LessThanOrEqual = endDate
                },
                Limit = 10,
            };
            var nextSet = _stripeApiWrapperService.InvoiceService.List(filters);

            invoices.AddRange(nextSet);
            while (nextSet.Any())
            {
                filters = new InvoiceListOptions()
                {
                    Status = "paid",
                    Created = new DateRangeOptions
                    {
                        GreaterThan = startDate,
                        LessThanOrEqual = endDate
                    },
                    Limit = 1,
                    StartingAfter = nextSet.Last().Id
                };
                nextSet = _stripeApiWrapperService.InvoiceService.List(filters);
                invoices.AddRange(nextSet);
            }
            return invoices;

        }
        public bool CancelSubscription(string cognitoId)
        {
            
            var user = _userService.GetUserByCognitoId(cognitoId);
            if (string.IsNullOrWhiteSpace(user.StripeCustomerId))
            {
                return false;
            }
            var subOptions = new SubscriptionListOptions() { Customer = user.StripeCustomerId };
            var subs = _stripeApiWrapperService.SubscriptionService.List(subOptions);
            if (subs.Count() > 1)
            {
                _logger.LogError("User had more than one subscription.... that doesn't seem right");
            }
            foreach (var sub in subs)
            {
                var cancelOptions = new SubscriptionCancelOptions
                {
                    InvoiceNow = false,
                    Prorate = true,
                };
                _stripeApiWrapperService.SubscriptionService.Cancel(sub.Id, cancelOptions);
            }
            return true;
        }
    }
}
