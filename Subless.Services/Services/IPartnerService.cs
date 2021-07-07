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
        IEnumerable<Partner> GetPartners();
        void UpdatePartner(Partner partner);
    }
}