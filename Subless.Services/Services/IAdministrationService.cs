using System;

namespace Subless.Services
{
    public interface IAdministrationService
    {
        void ActivateAdmin(Guid userId);
        void ActivateAdminWithKey(Guid submittedKey, string cognitoId);
        void OutputAdminKeyIfNoAdmins();
    }
}