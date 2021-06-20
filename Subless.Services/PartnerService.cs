using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public class PartnerService : IPartnerService
    {
        private IUserRepository _userRepository;
        public PartnerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            _userRepository.AddPartner(partner);
            return partner.Id;
        }


        public void UpdatePartner(Partner partner)
        {
            _userRepository.UpdatePartner(partner);
        }

        public List<Partner> GetPartners()
        {
            return _userRepository.GetPartners();
        }
    }
}
