using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache cache;

        public PartnerService(
            IUserRepository userRepository,
            IMemoryCache cache
            )
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public Partner GetPartner(Guid id)
        {
            return _userRepository.GetPartner(id);
        }

        public Guid GenerateCreatorActivationLink(string cognitoClientId, string creatorUsername)
        {
            var partner = _userRepository.GetPartnerByCognitoId(cognitoClientId);
            if (partner == null)
            {
                throw new UnauthorizedAccessException("Partner not found. Partner may not have been activated yet.");
            }
            var partnerId = partner.Id;
            var creator = _userRepository.GetCreatorByPartnerAndUsername(cognitoClientId, creatorUsername);
            if (creator == null)
            {
                creator = new Creator()
                {
                    PartnerId = partnerId,
                    Active = false,
                    Username = creatorUsername
                };
            }
            if (creator.Active)
            {
                throw new CreatorAlreadyActiveException();
            }
            var code = Guid.NewGuid();
            creator.ActivationCode = code;
            creator.ActivationExpiration = DateTime.UtcNow.AddMinutes(10);
            _userRepository.UpsertCreator(creator);
            return code;
        }

        public Guid CreatePartner(Partner partner)
        {
            partner.Site = new Uri(partner.Site.GetLeftPart(UriPartial.Authority));
            _userRepository.AddPartner(partner);
            return partner.Id;
        }


        public void UpdatePartner(Partner partner)
        {
            _userRepository.UpdatePartner(partner);
        }

        public IEnumerable<Partner> GetPartners()
        {
            return _userRepository.GetPartners();
        }

        public Partner GetCachedParnterByUri(Uri uri)
        {           
            if (cache.TryGetValue(uri.ToString(), out Partner partner))
            {
                return (Partner)cache.Get(uri.ToString());
            }
            partner = _userRepository.GetPartnerByUri(uri);
            cache.Set(uri.ToString(), partner, DateTime.UtcNow.AddHours(1));
            return partner;
        }
    }
}
