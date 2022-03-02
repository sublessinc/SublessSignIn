using Subless.Data;
using Subless.Models;
using SublessSignIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserService userService;
        private readonly IStripeService stripeService;
        private readonly ICreatorService creatorService;

        public AuthService(IUserRepository userRepository, IUserService userService, IStripeService stripeService, ICreatorService creatorService)
        {
            this._userRepo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.userService = userService;
            this.stripeService = stripeService;
            this.creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
        }

        public async Task<Redirection> LoginWorkflow(string cognitoId, string activationCode, string email)
        {
            var user = _userRepo.GetUserByCognitoId(cognitoId);
            if (user == null)
            {
                user = userService.CreateUserByCognitoId(cognitoId);
            }
            if (activationCode != null && Guid.TryParse(activationCode, out Guid code) && (user.Creators == null || !user.Creators.Any() || user.Creators.Any(x => !x.Active)))
            {
                await creatorService.ActivateCreator(user.Id, code, email);
                user = userService.GetUserWithRelationships(user.Id);
            }

            if ((user.Creators != null && user.Creators.Any(x => string.IsNullOrWhiteSpace(x.PayPalId))))
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.CreatorWithoutPayment
                };
            }
            if (user.Creators != null && user.Creators.Any())
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.ActivatedCreator
                };
            }

            if (!stripeService.CustomerHasPaid(cognitoId))
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

        public IEnumerable<RedirectionPath> GetAllowedPaths(string cognitoId)
        {
            var paths = new List<RedirectionPath>();
            var user = _userRepo.GetUserByCognitoId(cognitoId);
            if (user != null && user.StripeSessionId != null)
            {
                paths.Add(RedirectionPath.Profile);
            }
            if (user != null && user.StripeSessionId == null)
            {
                paths.Add(RedirectionPath.Payment);
            }
            if (user != null && user.Creators.Any())
            {
                paths.Add(RedirectionPath.ActivatedCreator);
            }
            if (user != null && user.Partners.Any())
            {
                paths.Add(RedirectionPath.Partner);
            }
            return paths;
        }
    }
}
