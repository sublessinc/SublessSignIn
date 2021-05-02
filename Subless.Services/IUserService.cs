using System;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public interface IUserService
    {
        RedirectionPath LoginWorkflow(string cognitoId);
        User CreateUserByCognitoId(string cognitoId);
        void AddStripeSessionId(string cognitoId, string stripeId);
        string GetStripeIdFromCognitoId(string cognitoId);
    }
}