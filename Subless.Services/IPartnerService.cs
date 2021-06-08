using System;

namespace Subless.Services
{
    public interface IPartnerService
    {
        Guid GenerateCreatorActivationLink(string cognitoClientId, string creatorUsername);
    }
}