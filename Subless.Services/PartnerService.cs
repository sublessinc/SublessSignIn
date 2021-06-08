using System;
using System.Collections.Generic;
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
            var partnerId = CreatePartnerIfNew(cognitoClientId);
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
            var code = Guid.NewGuid();
            creator.ActivationCode = code;
            creator.ActivationExpiration = DateTime.UtcNow.AddMinutes(10);
            _userRepository.SaveCreator(creator);
            return code;
        }

        public Guid CreatePartnerIfNew(string cognitoClientId)
        {
            var partner = _userRepository.GetPartnerByCognitoId(cognitoClientId);
            if (partner == null)
            {
                partner = new Partner()
                {
                    CognitoAppClientId = cognitoClientId
                };
                _userRepository.AddPartner(partner);
            }
            return partner.Id;
        }
    }
}
