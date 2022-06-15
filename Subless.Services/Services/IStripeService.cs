using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IStripeService
    {
        Task<bool> CanAccessStripe();
        bool CancelSubscription(string cognitoId);
        Task<CreateCheckoutSessionResponse> CreateCheckoutSession(long userBudget, string cognitoId);
        bool CustomerHasPaid(string cognitoId);
        List<Price> GetActiveSubscriptionPrice(string cognitoId);
        Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId);
        IEnumerable<Payer> GetInvoicesForRange(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<Stripe.Checkout.Session> GetSession(string sessionId);
        void RolloverPaymentForIdleCustomer(string customerId);
    }
}
