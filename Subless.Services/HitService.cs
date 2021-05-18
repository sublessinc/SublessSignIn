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
        UserRepository _userRepository;
        public HitService(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public void SaveHit(string userId, Uri uri)
        {
            _userRepository.SaveHit(new Hit()
            {
                UserId = userId,
                Uri = uri,
                TimeStamp = DateTime.Now
            });
        }
    }
}
