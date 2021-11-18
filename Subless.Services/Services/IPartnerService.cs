using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Services
{
    public interface IPartnerService
    {
        Guid CreatePartner(Partner partner);
        Task CreatorChangeWebhook(PartnerViewCreator creator);
        Guid GenerateCreatorActivationLink(string cognitoClientId, string creatorUsername);
        Partner GetCachedPartnerByUri(Uri uri);
        Partner GetPartner(Guid id);
        Partner GetPartnerByAdminId(Guid adminId);
        Partner GetPartnerByCognitoClientId(string cognitoId);
        IEnumerable<Partner> GetPartners();
        void UpdatePartner(Partner partner);
        Partner UpdatePartnerWritableFields(PartnerWriteModel partnerModel);
    }
}