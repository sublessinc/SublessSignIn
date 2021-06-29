using System;
using System.Collections.Generic;
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
        IEnumerable<User> GetAdmins();
        User GetUserById(Guid id);
        bool IsUserAdmin(string cognitoId);
        Creator GetCreatorByPartnerAndUsername(string partnerCognitoId, string username);
        IEnumerable<Partner> GetPartners();
        void UpdatePartner(Partner partner);
        void UpsertCreator(Creator creator);
        IEnumerable<Creator> GetCreatorsByCognitoId(string cognitoId);
        IEnumerable<User> GetUsersByCustomerIds(IEnumerable<string> customerIds);
        Creator GetCreator(Guid id);
        Partner GetPartner(Guid id);
        void SavePaymentLogs(IEnumerable<Payment> logs);
        void SavePaymentAuditLogs(IEnumerable<PaymentAuditLog> logs);
    }
}
