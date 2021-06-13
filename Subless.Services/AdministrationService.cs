using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Subless.Data;

namespace Subless.Services
{
    public class AdministrationService : IAdministrationService
    {
        IUserService _userService;
        ILogger _logger;
        IUserRepository _userRepository;
        public AdministrationService(IUserService userService, ILoggerFactory loggerFactory, IUserRepository userRepository)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = loggerFactory.CreateLogger<AdministrationService>();
        }

        public void OutputAdminKeyIfNoAdmins()
        {
            if (!_userService.GetAdmins().Any())
            {
                var key = Guid.NewGuid();
                _userRepository.SetAdminKey(key);
                _logger.LogError($"No admins configured, admin key: {key}");
            }
        }

        public void ActivateAdminWithKey(Guid submittedKey, string cognitoId)
        {
            var currentKey = _userRepository.GetAdminKey();
            if (currentKey == submittedKey)
            {
                var user = _userService.GetUserByCognitoId(cognitoId);
                _userService.SetUserAdmin(user.Id);
            }
        }

        public void ActivateAdmin(Guid userId)
        {
            _userService.SetUserAdmin(userId);
        }
    }
}
