using System;

namespace SublessSignIn.Models
{
    public class PartnerResponse
    {
        public string PayPalId { get; set; }
        public Uri[] Sites { get; set; }
        public string UserPattern { get; set; }
        public string CreatorWebhook { get; set; }
        public Guid Id { get; set; }
    }


}
