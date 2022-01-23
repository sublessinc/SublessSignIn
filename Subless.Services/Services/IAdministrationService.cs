using System;
using System.Threading.Tasks;

namespace Subless.Services
{
    public interface IAdministrationService
    {
        void ActivateAdmin(Guid userId);
        void ActivateAdminWithKey(Guid submittedKey, string cognitoId);
        Task<bool> CanAccessDatabase();
        void OutputAdminKeyIfNoAdmins();
    }
}