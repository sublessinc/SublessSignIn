using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Subless.Data
{
    public class UserRepository : DbContext, IUserRepository
    {
        private readonly IOptions<DatabaseSettings> _options;
        public DbSet<User> Users { get; set; }

        public UserRepository(IOptions<DatabaseSettings> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _ = options.Value.ConnectionString ?? throw new ArgumentNullException(nameof(options));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_options.Value.ConnectionString);
        }

        public User GetUserByStripeId(string id)
        {
            return Users.FirstOrDefault(x => x.StripeId == id);
        }

        public void AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
        }

        public void UpdateUser(User user)
        {
            Users.Update(user);
            SaveChanges();
        }
    }
}
