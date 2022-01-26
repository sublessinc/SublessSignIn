using Subless.Models;
using SublessSignIn.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SublessSignIn.Models
{
    public static class CreatorStatsExtensions
    {
        public static CreatorStats GetCreatorStats(this IEnumerable<Hit> hits)
        {
            if (hits is null)
            {
                throw new ArgumentNullException(nameof(hits));
            }
            var validHits = hits.Where(x => !string.IsNullOrWhiteSpace(x.CognitoId) && x.PartnerId != Guid.Empty);
            return new CreatorStats
            {
                Views = validHits.Count(),
                Visitors = validHits.Select(x => x.CognitoId).Distinct().Count(),
                PiecesOfContent = hits.Select(x => x.Uri).Distinct().Count(),
            };
        }
        public static HistoricalStats<CreatorStats> GetHistoricalCreatorStats(IEnumerable<Hit> thisMonth, IEnumerable<Hit> LastMonth)
        {
            return new HistoricalStats<CreatorStats>()
            {
                LastMonth = LastMonth.GetCreatorStats(),
                thisMonth = thisMonth.GetCreatorStats()
            };
        }
    }
}
