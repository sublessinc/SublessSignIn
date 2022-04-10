using Subless.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Data
{
    public interface IUserRepository
    {
        Guid AddUser(User user);
        Task<bool> CanAccessDatabase();
        void DeleteUser(User user);
        Guid? GetAdminKey();
        IEnumerable<User> GetAdmins();
        IEnumerable<Creator> GetCreatorsByCognitoId(string cognitoId);
        IEnumerable<Partner> GetPartnersByCognitoId(string cognitoId);
        User GetUserByCognitoId(string id);
        User GetUserById(Guid id);
        User GetUserByStripeId(string id);
        IEnumerable<User> GetUsersByCustomerIds(IEnumerable<string> customerIds);
        User GetUserWithRelationships(Guid id);
        bool IsUserAdmin(string cognitoId);
        void LogDbStats();
        void SetAdminKey(Guid? key);
        void UpdateUser(User user);
    }
}
