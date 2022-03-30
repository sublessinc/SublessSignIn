using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<Hit> GetValidHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, string cognitoId)
        {
            return Hits.Where(hit => hit.CognitoId == cognitoId
            && hit.CreatorId != Guid.Empty
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate).ToList();
        }

        public IEnumerable<Hit> GetCreatorHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid creatorId)
        {
            return Hits.Where(hit => hit.CreatorId == creatorId
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate).ToList();
        }

        public IEnumerable<Hit> GetPartnerHitsByDate(DateTimeOffset startDate, DateTimeOffset endDate, Guid partnerId)
        {
            return Hits.Where(hit => hit.PartnerId == partnerId
            && hit.TimeStamp > startDate
            && hit.TimeStamp <= endDate).ToList();
        }


    }
}
