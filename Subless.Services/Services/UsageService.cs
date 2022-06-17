using System;
using Subless.Data;
using Subless.Models;

namespace Subless.Services.Services
{
    public class UsageService : IUsageService
    {
        private readonly IUsageRepository _usageRepository;

        public UsageService(IUsageRepository usageRepository)
        {
            _usageRepository = usageRepository ?? throw new ArgumentNullException(nameof(usageRepository));
        }

        public void SaveUsage(UsageType type, Guid userId)
        {
            var stat = new UsageStat()
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                UsageType = type
            };

            _usageRepository.SaveUsageStat(stat);
        }
    }
}
