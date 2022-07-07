using Stripe;

namespace Subless.Services.Services
{
    public interface IStripeApiWrapperService
    {
        BalanceTransactionService BalanceTransactionService { get; }
        Stripe.BillingPortal.SessionService BillingSessionService { get; }
        ChargeService ChargeService { get; }
        CouponService CouponService { get; }
        CustomerService CustomerService { get; }
        InvoiceService InvoiceService { get; }
        PriceService PriceService { get; }
        RefundService RefundService { get; }
        Stripe.Checkout.SessionService SessionService { get; }
        SubscriptionService SubscriptionService { get; }
    }
}