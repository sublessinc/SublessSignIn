﻿using Subless.Models;

namespace SublessSignIn.Models
{
    public static class PartnerExtensions
    {
        public static PartnerResponse GetViewModel(this Partner partner)
        {
            return new PartnerResponse
            {
                PayoneerId = partner.PayoneerId,
                Site = partner.Site,
                UserPattern = partner.UserPattern
            };
        }
    }
}
