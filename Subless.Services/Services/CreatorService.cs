﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Subless.Data;
using Subless.Models;
using Subless.Services.Extensions;

namespace Subless.Services
{
    public class CreatorService : ICreatorService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPartnerService partnerService;
        private readonly IMemoryCache cache;
        private readonly ILogger<CreatorService> logger;

        public CreatorService(
            IUserRepository userRepository,
            IPartnerService partnerService,
            IMemoryCache cache,
            ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.partnerService = partnerService ?? throw new ArgumentNullException(nameof(partnerService));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.logger = loggerFactory?.CreateLogger<CreatorService>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task ActivateCreator(Guid userId, Guid activationCode, string email)
        {
            var creator = _userRepository.GetCreatorByActivationCode(activationCode);
            if (creator == null || creator.ActivationExpiration < DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAccessException("Activation code is invalid or expired");
            }
            creator.ActivationCode = null;
            creator.Active = true;
            creator.UserId = userId;
            creator.Email = email;
            _userRepository.UpdateCreator(creator);
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
            return _userRepository.GetCreator(id);
        }

        public IEnumerable<Creator> GetCreatorsByPartnerId(Guid partnerId)
        {
            return _userRepository.GetCreatorsByPartnerId(partnerId);
        }

        public Creator GetCachedCreatorFromPartnerAndUsername(string username, Guid partnerId)
        {
            var key = username + partnerId;
            if (cache.TryGetValue(key, out Creator creator))
            {
                return creator;
            }
            creator = _userRepository.GetCreatorByUsernameAndPartnerId(username, partnerId);
            cache.Set(key, creator, DateTimeOffset.UtcNow.AddHours(1));
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
            currentCreator.PayPalId = creator.PayPalId;
            _userRepository.UpdateCreator(currentCreator);
            await FireCreatorActivationWebhook(creator, wasValid);
            return currentCreator;
        }


        public IEnumerable<MontlyPaymentStats> GetStatsForCreator(Creator creator)
        {
            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }
            var payments = _userRepository.GetPaymentsByPayeePayPalId(creator.PayPalId);
            var paymentStats = new Dictionary<DateTimeOffset, MontlyPaymentStats>();
            foreach (var payment in payments)
            {
                var paymentMonth =  new DateTimeOffset(new DateTime(payment.DateSent.Year, payment.DateSent.Month, 1));
                if (!paymentStats.Keys.Any(x => new DateTimeOffset(new DateTime(x.Year, x.Month, 1)) == paymentMonth))
                {
                    paymentStats.Add(paymentMonth, new MontlyPaymentStats()
                    {
                        MonthStartDay = paymentMonth,
                    });
                }
                paymentStats[paymentMonth].DollarsPaid += (int)payment.Amount;
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
            return _userRepository.FilterInvalidCreators(creatorIds);
        }

        public async Task UnlinkCreator(string cognitoId, Guid id)
        {
            var creators = _userRepository.GetCreatorsByCognitoId(cognitoId);
            if (!creators.Any(x=>x.Id == id))
            {
                throw new AccessViolationException("User cannot modify this creator");
            }
            var creator = creators.Single(x => x.Id == id);
            _userRepository.DeleteCreator(creator);
            await partnerService.CreatorChangeWebhook(creator.ToPartnerView(true));
        }
    }
}
