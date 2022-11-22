using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subless.Data;
using Subless.Models;
using Subless.Services.Services.SublessStripe;

namespace Subless.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserService userService;
        private readonly IStripeService stripeService;
        private readonly ICreatorService creatorService;
        private readonly ITemplatedEmailService _templatedEmailService;
        private readonly ICognitoWrapper cognitoWrapper;

        public AuthService(
            IUserRepository userRepository, 
            IUserService userService, 
            IStripeService stripeService, 
            ICreatorService creatorService, 
            ITemplatedEmailService templatedEmailService,
            ICognitoWrapper cognitoWrapper)
        {
            _userRepo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
            this.creatorService = creatorService ?? throw new ArgumentNullException(nameof(creatorService));
            _templatedEmailService = templatedEmailService ?? throw new ArgumentNullException(nameof(templatedEmailService));
            this.cognitoWrapper = cognitoWrapper;
        }

        public async Task<Redirection> LoginWorkflow(string cognitoId, string activationCode, string email)
        {
            var hasPaid = stripeService.CachePaymentStatus(cognitoId);
            cognitoWrapper.SetEmail(cognitoId);
            var user = _userRepo.GetUserByCognitoId(cognitoId);
            if (user == null)
            {
                user = userService.CreateUserByCognitoId(cognitoId);
            }

            if (activationCode != null && Guid.TryParse(activationCode, out var code) && (user.Creators == null || !user.Creators.Any() || user.Creators.Any(x => !x.Active)))
            {
                await creatorService.ActivateCreator(user.Id, code, email);
                user = userService.GetUserWithRelationships(user.Id);
            }

            if (user.Creators != null && user.Creators.Any(x => x.AcceptedTerms == false))
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.CreatorTerms
                };
            }

            if (user.Creators != null && user.Creators.Any(x => string.IsNullOrWhiteSpace(x.PayPalId)))
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
            if (user.Partners != null && user.Partners.Any(x => x.AcceptedTerms == false))
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.PartnerTerms
                };
            }
            if (!user.AcceptedTerms)
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.Terms
                };
            }
            if (!hasPaid)
            {
                return new Redirection()
                {
                    RedirectionPath = RedirectionPath.Payment
                };
            }

            if (hasPaid && !user.WelcomeEmailSent)
            {
                _templatedEmailService.SendWelcomeEmail(user.CognitoId);
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
            var hasPaid = stripeService.CustomerHasPaid(cognitoId);
            if (hasPaid && !user.WelcomeEmailSent)
            {
                _templatedEmailService.SendWelcomeEmail(user.CognitoId);
            }

            if (user != null && hasPaid)
            {
                paths.Add(RedirectionPath.Profile);
            }
            if (user != null && !hasPaid)
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
