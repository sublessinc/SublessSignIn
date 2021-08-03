using System;
using System.Collections.Generic;
using System.Security.Claims;
using Subless.Models;
using SublessSignIn.Models;

namespace Subless.Services
{
    public interface IUserService
    {
        User CreateUserByCognitoId(string cognitoId);
        void AddStripeSessionId(string cognitoId, string stripeId);
        string GetStripeIdFromCognitoId(string cognitoId);
        User GetUserByCognitoId(string cognitoId);
        Redirection LoginWorkflow(string cognitoId, string activation);
        IEnumerable<User> GetAdmins();
        void SetUserAdmin(Guid userId);
        bool IsUserAdmin(string cognitoId);
        void AddStripeCustomerId(string cognitoId, string stripeId);
        IEnumerable<User> GetUsersFromStripeIds(IEnumerable<string> customerIds);
        User GetUser(Guid id);
        string GetUserClaim(ClaimsPrincipal user);
        IEnumerable<RedirectionPath> GetAllowedPaths(string cognitoId);
    }
}