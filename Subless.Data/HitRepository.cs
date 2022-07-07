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

        public IQueryable<Hit> GetValidHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId)
        {
            return Hits.Where(hit => hit.CognitoId == cognitoId
            && hit.CreatorId != Guid.Empty
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate);
        }

        public IQueryable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId)
        {
            return Hits.Where(hit => hit.CreatorId == creatorId
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate);
        }

        public IQueryable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId)
        {
            return Hits.Where(hit => hit.PartnerId == partnerId
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate);
        }

        public UserStats GetUserStats(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId)
        {
            var hits = GetValidHitsByDate(startDate, endDate, cognitoId);
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

        public CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId)
        {
            var hits = GetCreatorHitsByDate(startDate, endDate, creatorId);

            return new CreatorStats
            {
                PiecesOfContent = hits.Select(x => x.Uri).Distinct().Count(),
                Views = hits.Count(),
                Visitors = hits.Select(x => x.CognitoId).Distinct().Count(),
                PeriodEnd = endDate,
                PeriodStart = startDate,
            };
        }

        public PartnerStats GetPartnerStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId)
        {
            var hits = GetPartnerHitsByDate(startDate, endDate, partnerId);
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

        public List<HitView> GetRecentCreatorContent(Guid creatorId)
        {
            return Hits.Where(x => x.CreatorId == creatorId)
                .OrderByDescending(x => x.TimeStamp)
                .Select(x =>
                new HitView
                {
                    Content = x.Uri,
                    Title = x.Uri.Segments.Length > 1 ? x.Uri.Segments.Last() : x.Uri.Host,
                    Timestamp = x.TimeStamp.DateTime
                })
                .Take(5)
                .ToList();
        }

        public List<ContentHitCount> GetTopCreatorContent(Guid creatorId)
        {
            return Hits.Where(x => x.CreatorId == creatorId)
                .GroupBy(x => x.Uri)
                .Select(g =>
                new ContentHitCount
                {
                    Content = g.Key,
                    Title = g.Key.Segments.Length > 1 ? g.Key.Segments.Last() : g.Key.Host,
                    Hits = g.Count()
                })
                .OrderByDescending(x => x.Hits)
                .Take(5)
                .ToList();
        }
    }
}
