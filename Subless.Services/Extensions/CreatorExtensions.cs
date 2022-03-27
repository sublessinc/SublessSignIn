using Subless.Models;

namespace Subless.Services.Extensions
{
    public static class CreatorExtensions
    {
        public static PartnerViewCreator ToPartnerView(this Creator creator, bool deleted = false)
        {
            return new PartnerViewCreator()
            {
                PartnerId = creator.PartnerId,
                Active = creator.Active,
                Email = creator.Email,
                Id = creator.Id,
                Username = creator.Username,
                IsDeleted = deleted
            };
        }
    }
}
