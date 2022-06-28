using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Subless.Models;

namespace Subless.Services.Services
{
    public class StripeApiWrapperService : IStripeApiWrapperService
    {
        private readonly IOptions<StripeConfig> _stripeConfig;
        private readonly IStripeClient _client;
        private readonly SubscriptionService subscriptionService;
        private readonly CustomerService customerService;
        private readonly InvoiceService invoiceService;
        private readonly SessionService sessionService;
        private readonly BalanceTransactionService balanceTransactionService;
        private readonly PriceService priceService;
        private readonly CouponService couponService;
        private readonly Stripe.BillingPortal.SessionService billingSessionService;
        private readonly ChargeService chargeService;
        private readonly RefundService refundService;

        public SubscriptionService SubscriptionService => subscriptionService;

        public CustomerService CustomerService => customerService;

        public InvoiceService InvoiceService => invoiceService;

        public SessionService SessionService => sessionService;

        public BalanceTransactionService BalanceTransactionService => balanceTransactionService;

        public PriceService PriceService => priceService;

        public CouponService CouponService => couponService;

        public Stripe.BillingPortal.SessionService BillingSessionService => billingSessionService;

        public ChargeService ChargeService => chargeService;

        public RefundService RefundService => refundService;

        public StripeApiWrapperService(IOptions<StripeConfig> stripeConfig)
        {
            _stripeConfig = stripeConfig;
            _client = new StripeClient(_stripeConfig.Value.SecretKey ?? throw new ArgumentNullException(nameof(_stripeConfig.Value.SecretKey)));
            subscriptionService = new SubscriptionService(_client);
            customerService = new CustomerService(_client);
            invoiceService = new InvoiceService(_client);
            sessionService = new SessionService(_client);
            balanceTransactionService = new BalanceTransactionService(_client);
            priceService = new PriceService(_client);
            couponService = new CouponService(_client);
            billingSessionService = new Stripe.BillingPortal.SessionService(_client);
            chargeService = new ChargeService(_client);
            refundService = new RefundService(_client);
        }
    }
}
