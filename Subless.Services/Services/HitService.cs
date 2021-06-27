using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Data;
using Subless.Models;

namespace Subless.Services
{
    public class HitService : IHitService
    {
        private readonly IUserService _userService;
        private readonly UserRepository _userRepository;
        public HitService(IUserService userService, UserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public void SaveHit(string userId, Uri uri)
        {
            _userRepository.SaveHit(new Hit()
            {
                CognitoId = userId,
                Uri = uri,
                TimeStamp = DateTime.UtcNow
            });
        }

        public IEnumerable<Hit> GetHitsByDate(DateTime startDate, DateTime endDate, Guid userId)
        {
            var user = _userService.GetUser(userId);
            return _userRepository.GetHitsByDate(startDate, endDate, user.CognitoId);
        }
    }
}
