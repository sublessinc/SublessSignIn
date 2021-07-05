using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public class CreatorService : ICreatorService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache cache;

        public CreatorService(
            IUserRepository userRepository,
            IMemoryCache cache)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public void ActivateCreator(Guid userId, Guid activationCode)
        {
            var creator = _userRepository.GetCreatorByActivationCode(activationCode);
            if (creator == null || creator.ActivationExpiration < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Activation code is invalid or expired");
            }
            creator.ActivationCode = null;
            creator.Active = true;
            creator.UserId = userId;
            _userRepository.UpdateCreator(creator);
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
            cache.Set(key, creator, DateTime.UtcNow.AddHours(1));
            return creator;
        }

        public Creator UpdateCreator(string cognitoId, Creator creator)
        {
            var creators = _userRepository.GetCreatorsByCognitoId(cognitoId);
            if (creators == null || !creators.Any(x => x.Active))
            {
                throw new UnauthorizedAccessException();
            }
            // Set user modifiable properties
            var currentCreator = creators.First();
            currentCreator.PayoneerId = creator.PayoneerId;
            _userRepository.UpdateCreator(currentCreator);
            return currentCreator;
        }
    }
}
