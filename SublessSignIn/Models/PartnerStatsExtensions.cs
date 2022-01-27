using Subless.Models;
using SublessSignIn.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SublessSignIn.Models
{
    public static class PartnerStatsExtensions
    {
        public static PartnerStats GetPartnerStats(this IEnumerable<Hit> hits)
        {
            if (hits is null)
            {
                throw new ArgumentNullException(nameof(hits));
            }
            var validHits = hits.Where(x => x.CreatorId != Guid.Empty && x.PartnerId != Guid.Empty);
            return new PartnerStats
            {
                Views = validHits.Count(),
                Visitors = validHits.Select(x => x.CognitoId).Distinct().Count(),
                Creators = hits.Select(x => x.CreatorId).Distinct().Count(),
            };
        }
        public static HistoricalStats<PartnerStats> GetHistoricalPartnerStats(IEnumerable<Hit> thisMonth, IEnumerable<Hit> LastMonth)
        {
            return new HistoricalStats<PartnerStats>()
            {
                LastMonth = LastMonth.GetPartnerStats(),
                thisMonth = thisMonth.GetPartnerStats()
            };
        }
    }
}
