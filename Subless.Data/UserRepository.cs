using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Data
{
    public class UserRepository : DbContext, IUserRepository
    {
        private readonly IOptions<DatabaseSettings> _options;
        internal DbSet<User> Users { get; set; }
        internal DbSet<Hit> Hits { get; set; }
        internal DbSet<Partner> Partners { get; set; }
        internal DbSet<Creator> Creators { get; set; }

        public UserRepository(IOptions<DatabaseSettings> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _ = options.Value.ConnectionString ?? throw new ArgumentNullException(nameof(options));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_options.Value.ConnectionString);
        }

        public User GetUserByCognitoId(string id)
        {
            return Users.FirstOrDefault(x => x.CognitoId == id);
        }

        public User GetUserByStripeId(string id)
        {
            return Users.FirstOrDefault(x => x.StripeId == id);
        }

        public Guid AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
            return user.Id;
        }

        public void UpdateUser(User user)
        {
            Users.Update(user);
            SaveChanges();
        }

        public void SaveHit(Hit hit)
        {
            Hits.Add(hit);
            SaveChanges();
        }

        public Creator GetCreatorByActivationCode(Guid code)
        {
            return Creators.FirstOrDefault(creator => creator.ActivationCode == code);
        }

        public void SaveCreator(Creator creator)
        {
            Creators.Add(creator);
            SaveChanges();
        }

        public void UpdateCreator(Creator creator)
        {
            Creators.Update(creator);
            SaveChanges();
        }

        public void AddPartner(Partner partner)
        {
            Partners.Add(partner);
            SaveChanges();
        }

        public Partner GetPartnerByCognitoId(string partnerClientId)
        {
            return Partners.FirstOrDefault(x => x.CognitoAppClientId == partnerClientId);
        }
    }
}
