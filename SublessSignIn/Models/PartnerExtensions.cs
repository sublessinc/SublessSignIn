using Subless.Models;

namespace SublessSignIn.Models
{
    public static class PartnerExtensions
    {
        public static PartnerResponse GetViewModel(this Partner partner)
        {
            return new PartnerResponse
            {
                PayPalId = partner.PayPalId,
                Site = partner.Site,
                UserPattern = partner.UserPattern,
                CreatorWebhook = partner.CreatorWebhook?.ToString(),
                Id = partner.Id
            };
        }
    }
}
