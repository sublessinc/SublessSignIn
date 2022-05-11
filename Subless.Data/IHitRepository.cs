using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Subless.Data
{
    public interface IHitRepository
    {
        IQueryable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId);
        CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId);
        IQueryable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId);
        PartnerStats GetPartnerStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId);
        IQueryable<Hit> GetValidHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId);
        UserStats GetUserStats(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId);
        void LogDbStats();
        void SaveHit(Hit hit);
    }
}
