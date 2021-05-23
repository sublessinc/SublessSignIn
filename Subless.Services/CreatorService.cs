using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Data;

namespace Subless.Services
{
    public class CreatorService : ICreatorService
    {
        private IUserRepository _userRepository;
        public CreatorService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void ActivateCreator(Guid userId, Guid activationCode)
        {
            var creator = _userRepository.GetCreatorByActivationCode(activationCode);
            if (creator == null)
            {
                throw new UnauthorizedAccessException();
            }
            creator.ActivationCode = null;
            creator.Active = true;
            creator.UserId = userId;
            _userRepository.UpdateCreator(creator);
        }
    }
}
