using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subless.Models;

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
