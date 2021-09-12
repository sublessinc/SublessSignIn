using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public interface ICognitoService
    {
        Task DeleteCognitoUser(string cognitoUserId);
    }
}