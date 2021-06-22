﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Data;
using Subless.Models;

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
            if (creator == null || creator.ActivationExpiration < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Activation code is invalid or expired");
            }
            creator.ActivationCode = null;
            creator.Active = true;
            creator.UserId = userId;
            _userRepository.UpdateCreator(creator);
        }

        public Creator GetCreator(string cognitoId)
        {
            var creators = _userRepository.GetCreatorsByCognitoId(cognitoId);
            if (creators == null || !creators.Any(x => x.Active))
            {
                throw new UnauthorizedAccessException();
            }
            // TODO: One creator for now. 
            return creators.First();
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
