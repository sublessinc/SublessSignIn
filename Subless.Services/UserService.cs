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
        private IUserRepository _userRepo;
        private ICreatorService _creatorService;
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
            
            if (activationCode != null && Guid.TryParse(activationCode, out Guid code) && (user.Creators == null || !user.Creators.Any() || user.Creators.Any(x=>!x.Active)))
            {
                _creatorService.ActivateCreator(user.Id, code);
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.ActivatedCreator
                };
            }

            if (user.Creators!= null && user.Creators.Any())
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.ActivatedCreator
                };
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
            var user = GetUserByCognitoId(cognitoId);
            user.StripeId = stripeId;
            _userRepo.UpdateUser(user);
        }

        public User GetUserByCognitoId(string cognitoId)
        {
            return  _userRepo.GetUserByCognitoId(cognitoId);
        }

        public List<User> GetAdmins()
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
    }
}
