using System;
using Subless.Data;
using Subless.Models;
using SublessSignIn.Models;

namespace Subless.Services
{
    public interface IUserService
    {
        Redirection LoginWorkflow(string cognitoId);
        User CreateUserByCognitoId(string cognitoId);
        void AddStripeSessionId(string cognitoId, string stripeId);
        string GetStripeIdFromCognitoId(string cognitoId);
    }
}