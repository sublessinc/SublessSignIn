using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Data
{
    public class CreatorRepository : DbContext, ICreatorRepository
    {
        private readonly IOptions<DatabaseSettings> _options;
        private readonly ILogger logger;

        internal DbSet<Creator> Creators { get; set; }

        public CreatorRepository(IOptions<DatabaseSettings> options, ILoggerFactory loggerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            logger = loggerFactory?.CreateLogger<UserRepository>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void LogDbStats()
        {
            logger.LogWarning($"Creators count: {Creators.Count()}");
        }
        public void DeleteCreator(Creator creator)
        {
            Creators.Remove(creator);
            SaveChanges();
        }

        public Creator GetCreatorByActivationCode(Guid code)
        {
            return Creators.FirstOrDefault(creator => creator.ActivationCode == code);
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
            return Creators.FirstOrDefault(creator => creator.Username == username && creator.PartnerId == partnerId);
        }

        public void UpdateCreator(Creator creator)
        {
            Creators.Update(creator);
            SaveChanges();
        }

        public void SaveCreator(Creator creator)
        {
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
