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
    }
}
