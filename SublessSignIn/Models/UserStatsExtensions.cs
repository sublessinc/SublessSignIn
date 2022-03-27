using Subless.Models;
using SublessSignIn.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SublessSignIn.Models
{
    public static class UserStatsExtensions
    {
        public static UserStats GetUserStats(this IEnumerable<Hit> hits)
        {
            if (hits is null)
            {
                throw new ArgumentNullException(nameof(hits));
            }
            var validHits = hits.Where(x => x.CreatorId != Guid.Empty && x.PartnerId != Guid.Empty);
            return new UserStats
            {
                Views = validHits.Count(),
                Creators = validHits.Select(x => x.CreatorId).Distinct().Count(),
                Partners = hits.Select(x => x.PartnerId).Distinct().Count(),
            };
        }
        public static HistoricalStats<UserStats> GetHistoricalUserStats(IEnumerable<Hit> thisMonth, IEnumerable<Hit> LastMonth)
        {
            return new HistoricalStats<UserStats>()
            {
                LastMonth = LastMonth.GetUserStats(),
                thisMonth = thisMonth.GetUserStats()
            };
        }
    }
}
