using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IHitService
    {
        IEnumerable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId);
        IEnumerable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId);
        IEnumerable<Hit> GetHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid userId);
        bool SaveHit(string userId, Uri uri);
        Hit TestHit(string userId, Uri uri);
        UserStats GetUserStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid userId);
        CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId);
        PartnerStats GetPartnerStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId);
        IEnumerable<ContentHitCount> GetTopCreatorContent(Guid creatorId);
        IEnumerable<HitView> GetRecentCrecatorContent(Guid creatorId);
    }
}
