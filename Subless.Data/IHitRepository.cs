using Subless.Models;
using System;
using System.Collections.Generic;

namespace Subless.Data
{
    public interface IHitRepository
    {
        IEnumerable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId);
        IEnumerable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId);
        IEnumerable<Hit> GetValidHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId);
        void LogDbStats();
        void SaveHit(Hit hit);
    }
}