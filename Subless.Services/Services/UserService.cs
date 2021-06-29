using System;
using System.Collections.Generic;
using System.Linq;
using Subless.Data;
using Subless.Models;
using SublessSignIn.Models;

namespace Subless.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ICreatorService _creatorService;
        public UserService(IUserRepository userRepo, ICreatorService creatorService)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
        }

        public Redirection LoginWorkflow(string cognitoId, string activationCode)
        {
            var user = _userRepo.GetUserByCognitoId(cognitoId);
            if (user == null)
            {
                user = CreateUserByCognitoId(cognitoId);
            }

            if (activationCode != null && Guid.TryParse(activationCode, out Guid code) && (user.Creators == null || !user.Creators.Any() || user.Creators.Any(x => !x.Active)))
            {
                _creatorService.ActivateCreator(user.Id, code);
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.ActivatedCreator
                };
            }

            if (user.Creators != null && user.Creators.Any())
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.ActivatedCreator
                };
            }

            if (user.StripeSessionId == null)
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.Payment
                };
            }

            return new Redirection()
            {
                RedirectionPath = RedirectionPath.Profile,
                SessionId = user.StripeSessionId
            };
        }

        public string GetStripeIdFromCognitoId(string cognitoId)
        {
            return _userRepo.GetUserByCognitoId(cognitoId).StripeSessionId;
        }

        public IEnumerable<User> GetUsersFromStripeIds(IEnumerable<string> customerIds)
        {
            return _userRepo.GetUsersByCustomerIds(customerIds);
        }

        public User CreateUserByCognitoId(string cognitoId)
        {
            var user = new User()
            {
                CognitoId = cognitoId,
                StripeSessionId = null
            };
            user.Id = _userRepo.AddUser(user);
            return user;
        }

        public void AddStripeSessionId(string cognitoId, string stripeId)
        {
            var user = GetUserByCognitoId(cognitoId);
            user.StripeSessionId = stripeId;
            _userRepo.UpdateUser(user);
        }

        public void AddStripeCustomerId(string cognitoId, string stripeId)
        {
            var user = GetUserByCognitoId(cognitoId);
            user.StripeCustomerId = stripeId;
            _userRepo.UpdateUser(user);
        }

        public User GetUserByCognitoId(string cognitoId)
        {
            return _userRepo.GetUserByCognitoId(cognitoId);
        }

        public User GetUser(Guid id)
        {
            return _userRepo.GetUserById(id);
        }

        public IEnumerable<User> GetAdmins()
        {
            return _userRepo.GetAdmins();
        }

        public void SetUserAdmin(Guid userId)
        {
            var user = _userRepo.GetUserById(userId);
            user.IsAdmin = true;
            _userRepo.UpdateUser(user);
        }

        public bool IsUserAdmin(string cognitoId)
        {
            return _userRepo.IsUserAdmin(cognitoId);
        }

        public IEnumerable<User> GetUsersByStripeIds(IEnumerable<string> customerId)
        {
            throw new NotImplementedException();
        }
    }
}
