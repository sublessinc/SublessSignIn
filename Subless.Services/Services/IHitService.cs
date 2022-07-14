using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IHitService
    {
        IEnumerable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId);
        IEnumerable<Hit> GetHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid userId);
        bool SaveHit(string userId, Uri uri);
        Hit TestHit(string userId, Uri uri);
        UserStats GetUserStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid userId);
        CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId, string cognitoId);
        PartnerStats GetPartnerStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId);
        IEnumerable<ContentHitCount> GetTopCreatorContent(Guid creatorId, string cognitoId);
        IEnumerable<HitView> GetRecentCrecatorContent(Guid creatorId, string cognitoId);
        IEnumerable<Hit> FilterOutCreator(IEnumerable<Hit> hits, Guid creatorId);
    }
}
