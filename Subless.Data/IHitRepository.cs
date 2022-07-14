using System;
using System.Collections.Generic;
using System.Linq;
using Subless.Models;

namespace Subless.Data
{
    public interface IHitRepository
    {
        IQueryable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId, string cognitoId);
        CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId, string cognitoId);
        IQueryable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId);
        PartnerStats GetPartnerStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId);
        IQueryable<Hit> GetValidHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId, Guid? creatorId);
        UserStats GetUserStats(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId, Guid? creatorId);
        void LogDbStats();
        void SaveHit(Hit hit);
        List<ContentHitCount> GetTopCreatorContent(Guid creatorId, string cognitoId);
        List<HitView> GetRecentCreatorContent(Guid creatorId, string cognitoId);
    }
}
