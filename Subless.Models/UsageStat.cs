using System;

namespace Subless.Models
{
    public class UsageStat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Date { get; set; }
        public UsageType UsageType { get; set; }
    }

    public enum UsageType
    {
        UserStats = 0,
        CreatorStats = 1,
        PartnerStats = 2,
    }
}
