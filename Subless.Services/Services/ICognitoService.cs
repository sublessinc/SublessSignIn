using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface ICognitoService
    {
        Task DeleteCognitoUser(string cognitoUserId);
        Task<string> GetCognitoUserEmail(string cognitoUserId);
    }
}