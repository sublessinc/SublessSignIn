using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Services
{
    public interface IStripeService
    {
        Task<CreateCheckoutSessionResponse> CreateCheckoutSession(string priceId, string cognitoId);
        Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId);
        IEnumerable<Payer> GetInvoicesForRange(DateTime startDate, DateTime endDate);
        Task<Stripe.Checkout.Session> GetSession(string sessionId);
    }
}