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
        public Guid CreatePartnerLink(string cognitoClientId, string creatorUsername)
        {
            var partnerId = CreatePartnerIfNew(cognitoClientId);
            var code = Guid.NewGuid();
            var creator = new Creator()
            {
                PartnerId = partnerId,
                Active = false,
                ActivationCode = code,
                Username = creatorUsername
            };

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
