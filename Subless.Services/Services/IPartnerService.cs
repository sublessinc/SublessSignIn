using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services
{
    public interface IPartnerService
    {
        Guid CreatePartner(Partner partner);
        Guid GenerateCreatorActivationLink(string cognitoClientId, string creatorUsername);
        Partner GetCachedParnterByUri(Uri uri);
        Partner GetPartner(Guid id);
        Partner GetPartnerByAdminId(Guid adminId);
        IEnumerable<Partner> GetPartners();
        void UpdatePartner(Partner partner);
        Partner UpdatePartnerPayoneerId(Guid partnerId, string payoneerId);
    }
}