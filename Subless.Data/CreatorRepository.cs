using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Subless.Models;

namespace Subless.Data
{
    public partial class Repository : DbContext, ICreatorRepository
    {
        internal DbSet<Creator> Creators { get; set; }
        internal DbSet<CreatorMessage> CreatorMessages { get; set; }

        public void DeleteCreator(Creator creator)
        {
            Creators.Remove(creator);
            SaveChanges();
        }

        public CreatorMessage GetMessageForCreator(Guid creatorId)
        {
            return CreatorMessages.FirstOrDefault(x => x.CreatorId == creatorId && x.IsActive == true);
        }

        public void InvalidateCreatorMessages(Guid creatorId)
        {
            var oldMessages = CreatorMessages.Where(x => x.CreatorId == creatorId);
            foreach (var oldMessage in oldMessages)
            {
                oldMessage.IsActive = false;
            }
            CreatorMessages.UpdateRange(oldMessages);
            SaveChanges();
        }

        public CreatorMessage SetCreatorMessage(CreatorMessage message)
        {
            CreatorMessages.Add(message);
            SaveChanges();
            return message;
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

        public IEnumerable<Creator> GetActiveCreators(IEnumerable<Guid> excludedCreators = null)
        {
            var expression = Creators.Where(x => x.Active == true && x.PayPalId != null && x.AcceptedTerms == true);
            if (excludedCreators != null)
            {
                expression = expression.Where(x => !excludedCreators.Contains(x.Id));
            }
            return expression;
        }
    }
}
