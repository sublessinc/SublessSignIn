using System;
using Subless.Data;
using Subless.Models;
using SublessSignIn.Models;

namespace Subless.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        public Redirection LoginWorkflow(string cognitoId)
        {
            var user = _userRepo.GetUserByCognitoId(cognitoId);
            if (user == null)
            {
                user = CreateUserByCognitoId(cognitoId);
            }

            if (user.StripeId == null)
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.Payment
                };
            }

            return new Redirection()
            {
                RedirectionPath = RedirectionPath.Profile,
                SessionId = user.StripeId
            };
        }

        public string GetStripeIdFromCognitoId(string cognitoId)
        {
            return _userRepo.GetUserByCognitoId(cognitoId).StripeId;
        }

        public User CreateUserByCognitoId(string cognitoId)
        {
            var user = new User()
            {
                CognitoId = cognitoId,
                StripeId = null
            };
            user.Id= _userRepo.AddUser(user);
            return user;
        }

        public void AddStripeSessionId(string cognitoId, string stripeId)
        {
            var user = _userRepo.GetUserByCognitoId(cognitoId);
            user.StripeId = stripeId;
            _userRepo.UpdateUser(user);
        }
    }
}
