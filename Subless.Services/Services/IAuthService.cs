using System.Collections.Generic;
using Subless.Models;
using SublessSignIn.Models;

namespace Subless.Services.Services
{
    public interface IAuthService
    {
        IEnumerable<RedirectionPath> GetAllowedPaths(string cognitoId);
        Redirection LoginWorkflow(string cognitoId, string activationCode);
    }
}