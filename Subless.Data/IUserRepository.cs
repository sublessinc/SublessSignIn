using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Subless.Models;

namespace Subless.Data
{
    public interface IUserRepository
    {
        Guid AddUser(User user);
        User GetUserByStripeId(string id);
        void UpdateUser(User user);
        User GetUserByCognitoId(string id);
        void SaveCreator(Creator creator);
        void AddPartner(Partner partner);
        Creator GetCreatorByActivationCode(Guid code);
        void UpdateCreator(Creator creator);
        Partner GetPartnerByCognitoId(string partnerClientId);
        Guid? GetAdminKey();
        void SetAdminKey(Guid? key);
        List<User> GetAdmins();
        User GetUserById(Guid id);
        bool IsUserAdmin(string cognitoId);
        Creator GetCreatorByPartnerAndUsername(string partnerCognitoId, string username);
        List<Partner> GetPartners();
        void UpdatePartner(Partner partner);
    }
}
