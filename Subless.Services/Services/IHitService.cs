using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services
{
    public interface IHitService
    {
        IEnumerable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId);
        IEnumerable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId);
        IEnumerable<Hit> GetHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid userId);
        void SaveHit(string userId, Uri uri);
        Hit TestHit(string userId, Uri uri);
    }
}