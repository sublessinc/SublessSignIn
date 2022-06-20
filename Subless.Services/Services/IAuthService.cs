using System.Collections.Generic;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IAuthService
    {
        IEnumerable<RedirectionPath> GetAllowedPaths(string cognitoId);
        Task<Redirection> LoginWorkflow(string cognitoId, string activationCode, string email);
    }
}