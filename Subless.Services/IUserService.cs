using System;
using Subless.Data;
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
    }
}