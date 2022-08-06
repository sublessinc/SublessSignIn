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

namespace Subless.Services.Services.SublessStripe
{
    public class StripeService : IStripeService
    {
        private readonly IOptions<StripeConfig> _stripeConfig;
        private readonly IUserService _userService;
        private readonly IStripeApiWrapperServiceFactory _stripeApiWrapperServiceFactory;
        private readonly ILogger _logger;

        public StripeService(
            IOptions<StripeConfig> stripeConfig,
            IUserService userService,
            IStripeApiWrapperServiceFactory stripeApiWrapperServiceFactory,
            ILoggerFactory loggerFactory)
        {
            _stripeConfig = stripeConfig ?? throw new ArgumentNullException(nameof(stripeConfig));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _stripeApiWrapperServiceFactory = stripeApiWrapperServiceFactory ?? throw new ArgumentNullException(nameof(stripeApiWrapperServiceFactory));
            _logger = loggerFactory?.CreateLogger<StripeService>() ?? throw new ArgumentNullException(nameof(loggerFactory));

        }

        private void Release()
        {
            _stripeApiWrapperServiceFactory.Release();
        }

        public async Task<bool> CanAccessStripe()
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var list = service.CustomerService.ListAsync(new CustomerListOptions()
            {
                Limit = 1
            });

            Release();

            if (list != null)
            {
                return true;
            }
            return false;
        }

        public async Task<CreateCheckoutSessionResponse> CreateCheckoutSession(long userBudget, string cognitoId)
        {
            var priceId = await GetPriceIDByDollarAmount(userBudget);
            if (priceId == null)
            {
                _logger.LogCritical($"Price not found for budget of {userBudget}.");
                throw new ArgumentException("Invalid Price given.");
            }

            var user = _userService.GetUserByCognitoId(cognitoId);
            var customer = user.StripeCustomerId;

            if (!await CustomerHasPaid(cognitoId))
            {
                return await NewSubscription(user, priceId);
            }
            await UpgradeCustomer(customer, cognitoId, priceId);
            return null;

        }

        private async Task UpgradeCustomer(string customer, string cognitoId, string priceId)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var subs = await GetSubscriptions(customer);
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
                // This should defer plan changes to the beginning of next month
                ProrationBehavior = "none",
                Items = items,
            };
            service.SubscriptionService.Update(subscription.Id, options);
            Release();
        }

        private async Task<CreateCheckoutSessionResponse> NewSubscription(User user, string priceId)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            if (user.StripeCustomerId == null)
            {
                user.StripeCustomerId = (await CreateCustomer(user.CognitoId)).Id;
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
            var session = await service.SessionService.CreateAsync(options);
            _userService.AddStripeSessionId(user.CognitoId, session.Id);

            Release();

            return new CreateCheckoutSessionResponse
            {
                SessionId = session.Id,
            };
        }

        public async Task<Customer> CreateCustomer(string cognitoId)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var customerDetails = new CustomerCreateOptions
            {
                Description = cognitoId
            };
            var customer = service.CustomerService.Create(customerDetails);
            _userService.AddStripeCustomerId(cognitoId, customer.Id);

            Release();

            return customer;
        }

        private async Task<StripeList<Price>> GetPrices()
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            //TODO: productoptions should filter to only susbcription plans
            var productOptions = new PriceListOptions();
            var prices = service.PriceService.List(productOptions);

            Release();

            return prices;
        }

        private async Task<string> GetPriceIDByDollarAmount(long dollarAmount)
        {
            var prices = (await GetPrices()).ToList();
            //Stripe keeps the price in cents.
            var dollarAmountInCents = dollarAmount * 100;
            if (!prices.Any(x => x.UnitAmount == dollarAmountInCents))
            {
                CreatePriceByDollarAmount(dollarAmount);
                prices = (await GetPrices()).ToList();
            }
            var price = prices.Where(x => x.UnitAmount == dollarAmountInCents).Single();
            return price?.Id;
        }

        private async Task<string> CreatePriceByDollarAmount(long dollarAmount)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var price = service.PriceService.Create(new PriceCreateOptions()
            {
                Active = true,
                BillingScheme = "per_unit",
                Currency = "usd",
                Product = _stripeConfig.Value.CustomBudgetId,
                Recurring = new PriceRecurringOptions
                {
                    Interval = "month",
                    IntervalCount = 1,
                    UsageType = "licensed"
                },
                TaxBehavior = "unspecified",
                UnitAmount = dollarAmount * 100,
            });

            Release();

            return price?.Id;
        }

        public async Task RolloverPaymentForIdleCustomer(string customerId)
        {
            _logger.LogInformation($"Rolling over payment for idle customer {customerId}");
            var activeSubs = await GetSubscriptions(customerId);
            if (!activeSubs.Any())
            {
                _logger.LogInformation("Customer {custId} cancelled their sub before we could rollover their payment", customerId);
                return;
            }
            var sub = activeSubs.First(); //TODO figure out which sub to choose
            var coupon = await CreateOneTimeCoupon();
            await ApplyCouponToSubscription(coupon, sub);
        }

        private async Task<Coupon> CreateOneTimeCoupon()
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var options = new CouponCreateOptions
            {
                Duration = "once",
                Id = "rollover" + Guid.NewGuid(),
                PercentOff = 100,
                MaxRedemptions = 1
            };
            var result = service.CouponService.Create(options);

            Release();

            return result;

        }

        private async Task<Subscription> ApplyCouponToSubscription(Coupon coupon, Subscription sub)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var updateOptions = new SubscriptionUpdateOptions()
            {
                Coupon = coupon.Id,
            };
            var result = service.SubscriptionService.Update(sub.Id, updateOptions);

            Release();

            return result;
        }

        public async Task<bool> CustomerHasPaid(string cognitoId)
        {
            var activePrices = await GetActiveSubscriptionPriceId(cognitoId);
            var allPrices = await GetPrices();
            return allPrices.Any(x => activePrices.Contains(x.Id));
        }
        private async Task<List<string>> GetActiveSubscriptionPriceId(string cognitoId)
        {
            return (await GetActiveSubscriptionPrice(cognitoId)).Select(x => x.Id).ToList();
        }

        public async Task<List<Price>> GetActiveSubscriptionPrice(string cognitoId)
        {
            var prices = new List<Price>();
            var user = _userService.GetUserByCognitoId(cognitoId);
            if (user?.StripeCustomerId == null)
            {
                return prices;
            }

            var subscriptions = await GetSubscriptions(user.StripeCustomerId);

            foreach (var sub in subscriptions)
            {
                foreach (var item in sub.Items)
                {
                    prices.Add(item.Price);
                }
            }

            return prices;
        }

        private async Task<IEnumerable<Subscription>> GetSubscriptions(string stripeCustomerId)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var customer = service.CustomerService.Get(stripeCustomerId);
            var subscriptions = service.SubscriptionService.List(new SubscriptionListOptions()
            {
                Customer = customer.Id
            }).Where(sub => sub.Status == "active" && sub.CancelAtPeriodEnd == false);

            Release();

            return subscriptions;
        }

        public async Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            // TODO: Switch this to loading the session ID based on the cognito user id
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutSessionId = _userService.GetStripeIdFromCognitoId(cognitoId);
            var checkoutSession = await service.SessionService.GetAsync(checkoutSessionId);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = _stripeConfig.Value.Domain;

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = $"{returnUrl}/user-profile",
            };
            var result = await service.BillingSessionService.CreateAsync(options);
            Release();
            return result;
        }

        public async Task<Session> GetSession(string sessionId)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
            var result = await service.SessionService.GetAsync(sessionId);
            Release();
            return result;
        }

        public async Task<IEnumerable<Payer>> GetPayersForRange(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var utcutcStartDate = new DateTime(startDate.ToUniversalTime().Ticks, DateTimeKind.Utc);
            var utcutcEndDate = new DateTime(endDate.ToUniversalTime().Ticks, DateTimeKind.Utc);
            _logger.LogDebug($"Looking for invoices in time range UTC kind, UTC Time: {utcutcStartDate}  --  {utcutcEndDate}");
            //Stripe seems to convert datetimes to json and back, and use the local time when doing so. We need to pass a local time to prevent a double UTC conversion.
            List<Invoice> invoices;
            using (MiniProfiler.Current.Step("Get invoices"))
            {
                invoices = await GetInvoiceInRage(utcutcStartDate, utcutcEndDate);

            }
            var cusomterIds = invoices.Select(invoice => invoice.CustomerId);
            var users = _userService.GetUsersFromStripeIds(cusomterIds);
            var payers = new ConcurrentBag<Payer>();
            using (MiniProfiler.Current.Step("Per-payer data retrieval"))
            {
                // Parallel.ForEach(invoices, invoice =>
                // {
                //     var payer = await ProcessPayer(invoice, users);
                //     if (payer != null)
                //     {
                //         payers.Add(payer);
                //     }
                // });
            }
            return payers.ToList();
        }

        private async Task<Payer> ProcessPayer(Invoice invoice, IEnumerable<User> users)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();

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
                        charge = service.ChargeService.Get(invoice.ChargeId);

                    }
                    using (MiniProfiler.Current.Step("Get balance trans"))
                    {
                        balanceTrans = service.BalanceTransactionService.Get(charge.BalanceTransactionId);
                    }
                    fees = balanceTrans.Fee;
                    payment = balanceTrans.Net;
                    using (MiniProfiler.Current.Step("Get refunds"))
                    {
                        refunds = service.RefundService.List(new RefundListOptions() { Charge = invoice.ChargeId });
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

                Release();

                return new Payer
                {
                    UserId = users.Single(x => x.StripeCustomerId == invoice.CustomerId).Id,
                    Payment = payment,
                    Taxes = taxes,
                    Fees = fees
                };
            }

            Release();

            return null;
        }

        private async Task<List<Invoice>> GetInvoiceInRage(DateTime startDate, DateTime endDate)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();
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
            var nextSet = service.InvoiceService.List(filters);

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
                    Limit = 10,
                    StartingAfter = nextSet.Last().Id
                };
                nextSet = service.InvoiceService.List(filters);
                invoices.AddRange(nextSet);
            }

            Release();
            return invoices;

        }
        public async Task<bool> CancelSubscription(string cognitoId)
        {
            var service = await _stripeApiWrapperServiceFactory.GetAsync();

            var user = _userService.GetUserByCognitoId(cognitoId);
            if (string.IsNullOrWhiteSpace(user?.StripeCustomerId))
            {
                return false;
            }
            var subOptions = new SubscriptionListOptions() { Customer = user.StripeCustomerId };
            var subs = service.SubscriptionService.List(subOptions);
            if (subs.Count() > 1)
            {
                _logger.LogError("User had more than one subscription.... that doesn't seem right");
            }
            foreach (var sub in subs)
            {
                service.SubscriptionService.Update(sub.Id,
                    new SubscriptionUpdateOptions()
                    {
                        CancelAtPeriodEnd = true
                    });
            }

            Release();
            return true;
        }
    }
}
