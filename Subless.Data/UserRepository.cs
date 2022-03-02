using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Data
{
    public partial class Repository : DbContext, IUserRepository
    {
        internal DbSet<User> Users { get; set; }

        internal DbSet<RuntimeConfiguration> Configurations { get; set; }


        /// <summary>
        /// Just checks database connectivity
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanAccessDatabase()
        {
            var user = await Users.FirstOrDefaultAsync();
            return true;
        }

        public User GetUserByCognitoId(string id)
        {
            return Users.Include(i => i.Creators).Include(i => i.Partners).FirstOrDefault(x => x.CognitoId == id);
        }

        public IEnumerable<User> GetUsersByCustomerIds(IEnumerable<string> customerIds)
        {
            return Users.Where(x => customerIds.Contains(x.StripeCustomerId)).ToList();
        }

        public User GetUserByStripeId(string id)
        {
            return Users.FirstOrDefault(x => x.StripeSessionId == id);
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

        public void DeleteUser(User user)
        {
            Users.Remove(user);
            SaveChanges();
        }








        public IEnumerable<Creator> GetCreatorsByCognitoId(string cognitoId)
        {
            return Users.Include(x => x.Creators).FirstOrDefault(x => x.CognitoId == cognitoId)?.Creators?.ToList();
        }

        public void SetAdminKey(Guid? key)
        {
            if (Configurations.Any())
            {
                var config = Configurations.Single();
                config.AdminKey = key;
                Configurations.Update(config);
            }
            else
            {
                var config = new RuntimeConfiguration
                {
                    AdminKey = key
                };
                Configurations.Add(config);
            }
            SaveChanges();
        }



        public Guid? GetAdminKey()
        {
            return Configurations.FirstOrDefault()?.AdminKey;
        }

        public IEnumerable<User> GetAdmins()
        {
            return Users.Where(user => user.IsAdmin).ToList();
        }

        public User GetUserById(Guid id)
        {
            return Users.Find(id);
        }

        public User GetUserWithRelationships(Guid id)
        {
            return Users.Include(u => u.Creators).Include(u => u.Partners).Single(u => u.Id == id);
        }

        public bool IsUserAdmin(string cognitoId)
        {
            return Users.Any(x => x.CognitoId == cognitoId && x.IsAdmin);
        }



    }
}
