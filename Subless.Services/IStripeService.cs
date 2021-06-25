using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Services
{
    public interface IStripeService
    {
        Task<CreateCheckoutSessionResponse> CreateCheckoutSession(string priceId, string cognitoId);
        Task<Stripe.BillingPortal.Session> GetCustomerPortalLink(string cognitoId);
        Task<Stripe.Checkout.Session> GetSession(string sessionId);
    }
}