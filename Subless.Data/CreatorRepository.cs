using Microsoft.EntityFrameworkCore;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Subless.Data
{
    public partial class Repository : DbContext, ICreatorRepository
    {
        internal DbSet<Creator> Creators { get; set; }

        public void DeleteCreator(Creator creator)
        {
            Creators.Remove(creator);
            SaveChanges();
        }

        public Creator GetCreatorByActivationCode(Guid code)
        {
            return Creators.FirstOrDefault(creator => creator.ActivationCode == code);
        }

        public IEnumerable<Guid> FilterInvalidCreators(IEnumerable<Guid> creatorIds)
        {
            return Creators.Where(x => creatorIds.Contains(x.Id) && x.Active == true).Select(x => x.Id);
        }

        public Creator GetCreator(Guid id)
        {
            return Creators.Find(id);
        }

        public IEnumerable<Creator> GetCreatorsByPartnerId(Guid partnerId)
        {
            return Creators.Where(x => x.PartnerId == partnerId);
        }

        public Creator GetCreatorByUsernameAndPartnerId(string username, Guid partnerId)
        {
            return Creators.FirstOrDefault(creator => creator.Username == username && creator.PartnerId == partnerId && creator.Active);
        }

        public void UpdateCreator(Creator creator)
        {
            Creators.Update(creator);
            SaveChanges();
        }

        public void SaveCreator(Creator creator)
        {
            creator.CreateDate = DateTime.UtcNow;
            Creators.Add(creator);
            SaveChanges();
        }

        public void UpsertCreator(Creator creator)
        {
            if (creator.Id == Guid.Empty)
            {
                SaveCreator(creator);
            }
            else
            {
                UpdateCreator(creator);
            }
        }


    }
}
