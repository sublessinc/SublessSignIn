using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Subless.Data;
using Subless.Models;

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

        public string GetUserClaim(ClaimsPrincipal user)
        {
            return user.FindFirst("username")?.Value ?? user.FindFirst("cognito:username")?.Value;
        }

        public void ClearStripePayment(Guid id)
        {
            var user = _userRepo.GetUserById(id);
            user.StripeSessionId = null;
            _userRepo.UpdateUser(user);
        }

        public void DemoteUser(Guid id)
        {
            var user = _userRepo.GetUserWithRelationships(id);
            if (user.Partners != null && user.Partners.Any())
            {
                foreach (var partner in user.Partners)
                {
                    _userRepo.DeletePartner(partner);
                }
            }

            if (user.Creators != null && user.Creators.Any())
            {
                foreach (var creator in user.Creators)
                {
                    _userRepo.DeleteCreator(creator);
                }
            }

            if (user.IsAdmin)
            {
                user.IsAdmin = false;
                _userRepo.UpdateUser(user);
            }
        }
    }
}
