using System;
using System.Collections.Generic;
using System.Security.Claims;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IUserService
    {
        User CreateUserByCognitoId(string cognitoId);
        void AddStripeSessionId(string cognitoId, string stripeId);
        string GetStripeIdFromCognitoId(string cognitoId);
        User GetUserByCognitoId(string cognitoId);
        void AcceptTerms(string cognitoId);
        IEnumerable<User> GetAdmins();
        void SetUserAdmin(Guid userId);
        bool IsUserAdmin(string cognitoId);
        void AddStripeCustomerId(string user, string stripeId);
        IEnumerable<User> GetUsersFromStripeIds(IEnumerable<string> customerIds);
        User GetUser(Guid id);
        string GetUserClaim(ClaimsPrincipal user);
        void DemoteUser(Guid id);
        User GetUserWithRelationships(Guid id);
    }
}