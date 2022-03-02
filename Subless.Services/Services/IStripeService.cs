using Subless.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Services
{
    public interface IStripeService
    {
        Task<bool> CanAccessStripe();
        bool CancelSubscription(string cognitoId);
        Task<CreateCheckoutSessionResponse> CreateCheckoutSession(long userBudget, string cognitoId);
        bool CustomerHasPaid(string cognitoId);
        Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId);
        IEnumerable<Payer> GetInvoicesForRange(DateTimeOffset startDate, DateTimeOffset endDate);
        Task<Stripe.Checkout.Session> GetSession(string sessionId);
        void RolloverPaymentForIdleCustomer(string customerId);
    }
}