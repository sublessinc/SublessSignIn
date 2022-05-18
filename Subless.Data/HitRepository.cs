﻿using Microsoft.EntityFrameworkCore;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            };

        }

        public CreatorStats GetCreatorStats(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId)
        {
            var hits = GetCreatorHitsByDate(startDate, endDate, creatorId);

            return new CreatorStats
            {
                PiecesOfContent = hits.Select(x => x.Uri).Distinct().Count(),
                Views = hits.Count(),
                Visitors = hits.Select(x => x.CognitoId).Distinct().Count()
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
                Visitors = hits.Select(x => x.CognitoId).Distinct().Count()
            };
        }

        public List<Uri> GetRecentCreatorContent(Guid creatorId)
        {
            return Hits.Where(x => x.CreatorId == creatorId)
                .OrderByDescending(x => x.TimeStamp)
                .Select(x => x.Uri)
                .Take(5)
                .ToList();
        }

        public List<ContentHit> GetTopCreatorContent(Guid creatorId)
        {
            return Hits.Where(x => x.CreatorId == creatorId)
                .GroupBy(x => x.Uri)
                .Select(g => new ContentHit { Content= g.Key, Hits= g.Count() })
                .OrderBy(x=>x.Hits)
                .Take(5)
                .ToList();
        }
    }
}
