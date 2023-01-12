using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Subless.Models;

namespace Subless.Data
{
    public partial class Repository : DbContext, IHitRepository
    {

        internal DbSet<Hit> Hits { get; set; }

        public void SaveHit(Hit hit)
        {
            Hits.Add(hit);
            SaveChanges();
        }

        public IQueryable<Hit> GetValidHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId, Guid? creatorId)
        {
            var hits = Hits.Where(hit => hit.CognitoId == cognitoId
            && hit.CreatorId != Guid.Empty
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate
            && hit.CreatorId != creatorId);
            return hits;
        }

        public IQueryable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, IEnumerable<Guid> creatorIds, string cognitoId)
        {
            var hits = Hits.Where(hit => creatorIds.Contains(hit.CreatorId)
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate
            && hit.CognitoId != cognitoId);
            return hits;
        }

        public IQueryable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId)
        {
            var hits = Hits.Where(hit => hit.PartnerId == partnerId
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate
            && hit.CognitoId != cognitoId);
            return hits;
        }

        public UserStats GetUserStats(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId, Guid? creatorId)
        {
            var hits = GetValidHitsByDate(startDate, endDate, cognitoId, creatorId);
            var distinctCreators = hits.Select(x => x.CreatorId).Distinct();
            return new UserStats
            {
                Views = hits.Count(),
                Creators = Creators.Where(creator => distinctCreators.Contains(creator.Id) && creator.Active).Count(),
                Partners = hits.Select(x => x.PartnerId).Distinct().Count(),
                PeriodEnd = endDate,
                PeriodStart = startDate
            };
        }

        public CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, IEnumerable<Guid> creatorIds, string cognitoId)
        {
            var hits = GetCreatorHitsByDate(startDate, endDate, creatorIds, cognitoId);

            return new CreatorStats
            {
                PiecesOfContent = hits.Select(x => x.Uri).Distinct().Count(),
                Views = hits.Count(),
                Visitors = hits.Select(x => x.CognitoId).Distinct().Count(),
                PeriodEnd = endDate,
                PeriodStart = startDate,
            };
        }

        public PartnerStats GetPartnerStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId, string cognitoId)
        {
            var hits = GetPartnerHitsByDate(startDate, endDate, partnerId, cognitoId);
            var distinctCreators = hits.Select(x => x.CreatorId).Distinct();

            return new PartnerStats()
            {
                Creators = Creators.Where(creator => distinctCreators.Contains(creator.Id) && creator.Active).Count(),
                Views = hits.Count(),
                Visitors = hits.Select(x => x.CognitoId).Distinct().Count(),
                PeriodStart = startDate,
                PeriodEnd = endDate,
            };
        }

        public List<HitView> GetRecentCreatorContent(Guid creatorId, string cognitoId)
        {
            var hits = Hits.Where(x => x.CreatorId == creatorId && x.CognitoId != cognitoId)
                .OrderByDescending(x => x.TimeStamp)
                .Select(x =>
                new HitView
                {
                    Content = x.Uri,
                    Title = x.Uri.Segments.Length > 1 ? x.Uri.Segments.Last() : x.Uri.Host,
                    Timestamp = x.TimeStamp.DateTime,
                    PartnerId = x.PartnerId
                })
                .Take(5)
                .ToList();

            hits = (List<HitView>)EnrichFavicons(hits);
            return hits;
        }

        public List<ContentHitCount> GetTopCreatorContent(Guid creatorId, string cognitoId)
        {
            var hits = Hits.Where(x => x.CreatorId == creatorId && x.CognitoId != cognitoId)
                .GroupBy(x => new { x.Uri, x.PartnerId })
                .Select(g =>
                new ContentHitCount
                {
                    Content = g.Key.Uri,
                    Title = g.Key.Uri.Segments.Length > 1 ? g.Key.Uri.Segments.Last() : g.Key.Uri.Host,
                    Hits = g.Count(),
                    PartnerId = g.Key.PartnerId
                })
                .OrderByDescending(x => x.Hits)
                .Take(5)
                .ToList();

            hits = (List<ContentHitCount>)EnrichFavicons(hits);
            return hits;
        }


        public List<HitView> GetRecentPatronContent(string cognitoId, Guid? creatorId = null)
        {
            var hits = Hits.Where(x =>
            x.CognitoId == cognitoId &&
            x.CreatorId != creatorId &&
            x.CreatorId != Guid.Empty)
                .OrderByDescending(x => x.TimeStamp)
                .Select(x =>
                new HitView
                {
                    Content = x.Uri,
                    Title = x.Uri.Segments.Length > 1 ? x.Uri.Segments.Last() : x.Uri.Host,
                    Timestamp = x.TimeStamp.DateTime,
                    PartnerId = x.PartnerId
                })
                .Take(5)
                .ToList();

            hits = (List<HitView>)EnrichFavicons(hits);

            return hits;
        }

        public List<CreatorHitCount> GetTopPatronContent(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId, Guid? creatorId = null)
        {
            var totalHits = Hits.Where(x =>
            x.CognitoId == cognitoId &&
            x.CreatorId != creatorId &&
            x.CreatorId != Guid.Empty &&
            x.TimeStamp > startDate &&
            x.TimeStamp < endDate).Count();

            var hits = Hits.Where(x =>
            x.CognitoId == cognitoId &&
            x.CreatorId != creatorId &&
            x.CreatorId != Guid.Empty &&
            x.TimeStamp > startDate &&
            x.TimeStamp < endDate)
                .GroupBy(x => new { x.CreatorId, x.PartnerId } )
                .Select(g =>
                new CreatorHitCount
                {
                    CreatorId = g.Key.CreatorId,
                    Hits = (int)Math.Floor(100 * (decimal)g.Count() / totalHits),
                    PartnerId = g.Key.PartnerId,
                })
                .OrderByDescending(x => x.Hits)
                .Take(5)
                .ToList();

            hits = (List<CreatorHitCount>)EnrichFavicons(hits);

            return hits;
        }

        private IEnumerable<IFaviconable> EnrichFavicons(IEnumerable<IFaviconable> items) 
        {
            foreach (var hit in items)
            {
                hit.Favicon = GetPartner(hit.PartnerId).Favicon;
            }
            return items;
        }
    }
}
