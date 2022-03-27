using Subless.Models;
using System;
using System.Collections.Generic;

namespace Subless.Data
{
    public interface IPartnerRepository
    {
        void AddPartner(Partner partner);
        void DeletePartner(Partner partner);
        Creator GetCreatorByPartnerAndUsername(string partnerCognitoId, string username);
        Partner GetPartner(Guid id);
        Partner GetPartnerByAdminId(Guid id);
        Partner GetPartnerByCognitoId(string partnerClientId);
        Partner GetPartnerByUri(Uri uri);
        IEnumerable<Partner> GetPartners();
        IEnumerable<Uri> GetPartnerUris();
        void LogDbStats();
        void UpdatePartner(Partner partner);
    }
}