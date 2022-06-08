using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.Models;
using Subless.Services.Extensions;
using Subless.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subless.Services
{
    public class CreatorService : ICreatorService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICreatorRepository creatorRepository;
        private readonly IPartnerService partnerService;
        private readonly ICacheService cache;
        private readonly ILogger<CreatorService> logger;
        private readonly IPaymentRepository paymentRepository;
        private readonly IEmailService _emailService;

        public CreatorService(
            IUserRepository userRepository,
            ICreatorRepository creatorRepository,
            IPartnerService partnerService,
            IPaymentRepository paymentRepository,
            IPaymentLogsService logsService,
            IEmailService emailService,
            ICacheService cache,
            ILoggerFactory loggerFactory)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.creatorRepository = creatorRepository ?? throw new ArgumentNullException(nameof(creatorRepository));
            this.partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            this.paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.logger = loggerFactory?.CreateLogger<CreatorService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task ActivateCreator(Guid userId, Guid activationCode, string email)
        {
            var creator = creatorRepository.GetCreatorByActivationCode(activationCode);
            if (creator == null || creator.ActivationExpiration < DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAccessException("Activation code is invalid or expired");
            }
            creator.ActivationCode = null;
            creator.Active = true;
            creator.UserId = userId;
            creator.Email = email;
            creatorRepository.UpdateCreator(creator);
            await FireCreatorActivationWebhook(creator, false);
        }

        public Creator GetCreatorByCognitoid(string cognitoId)
        {
            var creators = _userRepository.GetCreatorsByCognitoId(cognitoId);
            if (creators == null || !creators.Any(x => x.Active))
            {
                throw new UnauthorizedAccessException();
            }
            // TODO: One creator for now.
            return creators.First();
        }

        public Creator GetCreator(Guid id)
        {
            return creatorRepository.GetCreator(id);
        }

        public IEnumerable<Creator> GetCreatorsByPartnerId(Guid partnerId)
        {
            return creatorRepository.GetCreatorsByPartnerId(partnerId);
        }

        public Creator GetCachedCreatorFromPartnerAndUsername(string username, Guid partnerId)
        {
            var key = username + partnerId;
            if (cache.Cache.TryGetValue(key, out Creator creator))
            {
                return creator;
            }
            creator = creatorRepository.GetCreatorByUsernameAndPartnerId(username, partnerId);
            cache.Cache.Set(key, creator, DateTimeOffset.UtcNow.AddSeconds(15));
            return creator;
        }

        public async Task<Creator> UpdateCreator(string cognitoId, Creator creator)
        {
            var creators = _userRepository.GetCreatorsByCognitoId(cognitoId);
            if (creators == null || !creators.Any(x => x.Active))
            {
                throw new UnauthorizedAccessException();
            }
            // Set user modifiable properties
            var currentCreator = creators.First();
            var wasValid = CreatorValid(currentCreator);
            if (currentCreator.PayPalId != null && currentCreator.PayPalId!=creator.PayPalId)
            {
                await _emailService.SendEmail(GetPaymentChangedEmail(creator.Username), currentCreator.PayPalId, "Subless payout no longer associated with this email");
            }
            currentCreator.PayPalId = creator.PayPalId;
            creatorRepository.UpdateCreator(currentCreator);
            await _emailService.SendEmail(GetPaymentSetEmail(creator.Username), creator.PayPalId, "Subless payout email set");
            await FireCreatorActivationWebhook(creator, wasValid);
            return currentCreator;
        }


        public IEnumerable<MontlyPaymentStats> GetStatsForCreator(Creator creator)
        {
            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }
            var payments = paymentRepository.GetPaymentsByPayeePayPalId(creator.PayPalId);
            var paymentStats = new Dictionary<DateTimeOffset, MontlyPaymentStats>();
            foreach (var payment in payments)
            {
                var paymentMonth = new DateTimeOffset(new DateTime(payment.DateSent.Year, payment.DateSent.Month, 1));
                if (!paymentStats.Keys.Any(x => new DateTimeOffset(new DateTime(x.Year, x.Month, 1)) == paymentMonth))
                {
                    paymentStats.Add(paymentMonth, new MontlyPaymentStats()
                    {
                        MonthStartDay = paymentMonth,
                    });
                }
                paymentStats[paymentMonth].DollarsPaid += Math.Round(payment.Amount/100, 2);
                paymentStats[paymentMonth].Payers += 1;
            }
            return paymentStats.Values.OrderBy(x => x.MonthStartDay);
        }

        private bool CreatorValid(Creator creator)
        {
            return (
                creator.Active &&
                creator.PayPalId != null
                );
        }

        public async Task FireCreatorActivationWebhook(Creator creator, bool wasValid)
        {
            logger.LogInformation("Checking if webhook should fire");
            var isValid = CreatorValid(creator);
            logger.LogInformation($"Creator was valid {wasValid}");
            logger.LogInformation($"Creator is valid {isValid}");
            if (isValid != wasValid)
            {
                await partnerService.CreatorChangeWebhook(creator.ToPartnerView());
            }
        }

        public IEnumerable<Guid> FilterInactiveCreators(IEnumerable<Guid> creatorIds)
        {
            return creatorRepository.FilterInvalidCreators(creatorIds);
        }

        public async Task UnlinkCreator(string cognitoId, Guid id)
        {
            var creators = _userRepository.GetCreatorsByCognitoId(cognitoId);
            if (!creators.Any(x => x.Id == id))
            {
                throw new UnauthorizedAccessException("User cannot modify this creator");
            }
            var creator = creators.Single(x => x.Id == id);
            creatorRepository.DeleteCreator(creator);
            cache.InvalidateCache();
            await partnerService.CreatorChangeWebhook(creator.ToPartnerView(true));
        }

        public void AcceptTerms(string cognitoId)
        {
            var creators = _userRepository.GetCreatorsByCognitoId(cognitoId);
            var creator = creators.Single();
            creator.AcceptedTerms = true;
            creatorRepository.UpdateCreator(creator);
        }

        private string GetPaymentSetEmail(string creatorName)
        {
            return $"The subless creator {creatorName} has set their payout to be sent to this address. If this was a mistake, please visit your profile to change it. " +
                $"If you do not have a subless account, please reach out to contact@subless.com for help.";
        }

        private string GetPaymentChangedEmail(string creatorName)
        {
            return $"The subless creator {creatorName} has changed their payout to no longer be this address. If this was a mistake, please visit your profile to change it. " +
                $"If you did not perform this action, please reach out to contact@subless.com for help.";
        }
    }
}
