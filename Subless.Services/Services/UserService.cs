using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Subless.Data;
using Subless.Models;

namespace Subless.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ICreatorRepository creatorRepo;
        private readonly IPartnerRepository partnerRepo;
        public UserService(
            IUserRepository userRepo,
            ICreatorRepository creatorRepo,
            IPartnerRepository partnerRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            this.creatorRepo = creatorRepo ?? throw new ArgumentNullException(nameof(creatorRepo));
            this.partnerRepo = partnerRepo ?? throw new ArgumentNullException(nameof(partnerRepo));
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

        public void WelcomeSent(string cognitoId)
        {
            var user = GetUserByCognitoId(cognitoId);
            user.WelcomeEmailSent = true;
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

        public User GetUserWithRelationships(Guid id)
        {
            return _userRepo.GetUserWithRelationships(id);

        }

        public void DemoteUser(Guid id)
        {
            var user = GetUserWithRelationships(id);
            if (user.Partners != null && user.Partners.Any())
            {
                foreach (var partner in user.Partners)
                {
                    partnerRepo.DeletePartner(partner);
                }
            }

            if (user.Creators != null && user.Creators.Any())
            {
                foreach (var creator in user.Creators)
                {
                    creatorRepo.DeleteCreator(creator);
                }
            }

            if (user.IsAdmin)
            {
                user.IsAdmin = false;
                _userRepo.UpdateUser(user);
            }
        }

        public IEnumerable<string> GetAllCognitoIds()
        {
            return _userRepo.GetAllCognitoIds();
        }

        public void AcceptTerms(string cognitoId)
        {
            var user = _userRepo.GetUserByCognitoId(cognitoId);
            user.AcceptedTerms = true;
            _userRepo.UpdateUser(user);
        }

        public void CachePaymentStatus(string cognitoId, bool isPaying, long? activeSubscriptionPrice, DateTimeOffset? subStartDate)
        {
            _userRepo.CachePaymentStatus(cognitoId, isPaying, activeSubscriptionPrice, subStartDate);
        }

        public void UpdateUser(User user)
        {
            _userRepo.UpdateUser(user);
        }
    }
}
