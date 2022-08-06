using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Subless.Models;

namespace Subless.Services.Services.SublessStripe
{
    public interface IStripeService
    {
        Task<bool> CanAccessStripe();
        Task<bool> CancelSubscription(string cognitoId);
        Task<CreateCheckoutSessionResponse> CreateCheckoutSession(long userBudget, string cognitoId);
        Task<bool> CustomerHasPaid(string cognitoId);
        Task<List<Price>> GetActiveSubscriptionPrice(string cognitoId);
        Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId);
        Task<IEnumerable<Payer>> GetPayersForRange(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<Stripe.Checkout.Session> GetSession(string sessionId);
        Task RolloverPaymentForIdleCustomer(string customerId);
    }
}
