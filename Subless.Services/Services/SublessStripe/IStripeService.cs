using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe
{
    public interface IStripeService
    {
        bool CachePaymentStatus(string cognitoId);
        Task<bool> CanAccessStripe();
        bool CancelSubscription(string cognitoId);
        Task<CreateCheckoutSessionResponse> CreateCheckoutSession(long userBudget, string cognitoId);
        bool CustomerHasPaid(string cognitoId);
        List<Price> GetActiveSubscriptionPrice(string cognitoId);
        Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId);
        IEnumerable<Payer> GetPayersForRange(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<Stripe.Checkout.Session> GetSession(string sessionId);
        void RolloverPaymentForIdleCustomer(string customerId);
    }
}
