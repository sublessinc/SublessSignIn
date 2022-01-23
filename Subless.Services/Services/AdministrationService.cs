using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Subless.Data;

namespace Subless.Services
{
    public class AdministrationService : IAdministrationService
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
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

        public async Task<bool> CanAccessDatabase()
        {
            return await _userRepository.CanAccessDatabase();
        }

        public void OutputAdminKeyIfNoAdmins()
        {
            var admins = _userService.GetAdmins();
            if (!admins.Any())
            {
                var key = Guid.NewGuid();
                _userRepository.SetAdminKey(key);
                _logger.LogError($"No admins configured, admin key: {key}");
            }
            else
            {
                _logger.LogInformation($"Admins: {string.Join("\n", admins.Select(x => x.CognitoId))}");
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
