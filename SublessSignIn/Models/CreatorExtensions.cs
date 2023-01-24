using System;
using Subless.Models;
namespace SublessSignIn.Models
{
    public static class CreatorExtensions
    {
        public static CreatorViewModel GetViewModel(this Creator creator, Uri PartnerSite = null)
        {
            return new CreatorViewModel
            {
                PayPalId = creator.PayPalId,
                Username = creator.Username,
                PartnerUri = PartnerSite,
                Id = creator.Id
            };
        }
    }
}
