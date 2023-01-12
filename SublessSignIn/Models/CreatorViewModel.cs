using System;

namespace SublessSignIn.Models
{
    public class CreatorViewModel
    {
        public string PayPalId { get; set; }
        public string Username { get; set; }
        public Guid Id { get; set; }
        public Uri PartnerUri { get; set; }
    }
}
