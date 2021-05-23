using System;

namespace Subless.Services
{
    public interface IPartnerService
    {
        Guid CreatePartnerLink(string cognitoClientId, string creatorUsername);
    }
}