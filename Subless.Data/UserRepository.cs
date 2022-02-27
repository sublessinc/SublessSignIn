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
    public class UserRepository : DbContext, IUserRepository
    {
        private readonly IOptions<DatabaseSettings> _options;
        internal DbSet<User> Users { get; set; }
        internal DbSet<Hit> Hits { get; set; }
        internal DbSet<Partner> Partners { get; set; }
        internal DbSet<Creator> Creators { get; set; }
        internal DbSet<Payment> Payments { get; set; }
        internal DbSet<PaymentAuditLog> PaymentAuditLogs { get; set; }
        internal DbSet<RuntimeConfiguration> Configurations { get; set; }
        ILogger<UserRepository> logger { get; set; }
        public UserRepository(IOptions<DatabaseSettings> options, ILoggerFactory loggerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _ = options.Value.ConnectionString ?? throw new ArgumentNullException(nameof(options));
            logger = loggerFactory?.CreateLogger<UserRepository>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Just checks database connectivity
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanAccessDatabase()
        {
            var user = await Users.FirstOrDefaultAsync();
            return true;
        }

        public void LogDbStats()
        {
            logger.LogWarning($"User count: {Users.Count()}");
            logger.LogWarning($"Hits count: {Hits.Count()}");
            logger.LogWarning($"Partners count: {Partners.Count()}");
            logger.LogWarning($"Creators count: {Creators.Count()}");
            logger.LogWarning($"Payments count: {Payments.Count()}");
            logger.LogWarning($"PaymentAuditLogs count: {PaymentAuditLogs.Count()}");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_options.Value.ConnectionString);
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

        public void DeleteCreator(Creator creator)
        {
            Creators.Remove(creator);
            SaveChanges();
        }

        public void DeletePartner(Partner partner)
        {
            Partners.Remove(partner);
            SaveChanges();
        }

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

        public Creator GetCreatorByActivationCode(Guid code)
        {
            return Creators.FirstOrDefault(creator => creator.ActivationCode == code);
        }

        public IEnumerable<Creator> GetCreatorsByCognitoId(string cognitoId)
        {
            return Users.Include(x => x.Creators).FirstOrDefault(x => x.CognitoId == cognitoId)?.Creators?.ToList();
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

        public Partner GetPartnerByUri(Uri uri)
        {
            return Partners.FirstOrDefault(partner => partner.Site == uri);
        }

        public void UpdateCreator(Creator creator)
        {
            Creators.Update(creator);
            SaveChanges();
        }

        public Creator GetCreatorByPartnerAndUsername(string partnerCognitoId, string username)
        {
            var partners = Partners.Include(p => p.Creators).Where(x => x.CognitoAppClientId == partnerCognitoId);
            var partner = partners.Single();
            var creator = partner.Creators.FirstOrDefault(y => y.Username == username);
            return creator;
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

        public void AddPartner(Partner partner)
        {
            Partners.Add(partner);
            SaveChanges();
        }

        public void UpdatePartner(Partner partner)
        {
            Partners.Update(partner);
            SaveChanges();
        }

        public IEnumerable<Partner> GetPartners()
        {
            return Partners.Include(p=>p.Creators).ToList();
        }

        public Partner GetPartner(Guid id)
        {
            return Partners.Find(id);
        }

        public Partner GetPartnerByAdminId(Guid id)
        {
            return Partners.FirstOrDefault(x => x.Admin == id);
        }

        public Partner GetPartnerByCognitoId(string partnerClientId)
        {
            return Partners.FirstOrDefault(x => x.CognitoAppClientId == partnerClientId);
        }

        public IEnumerable<Guid> FilterInvalidCreators(IEnumerable<Guid> creatorIds)
        {
            return Creators.Where(x => creatorIds.Contains(x.Id) && x.Active == true).Select(x => x.Id);
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

        public void SavePaymentLogs(IEnumerable<Payment> logs)
        {
            Payments.AddRange(logs);
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

        public void SavePaymentAuditLogs(IEnumerable<PaymentAuditLog> logs)
        {
            PaymentAuditLogs.AddRange(logs);
            SaveChanges();
        }

        public IEnumerable<Payment> GetPaymentsByPayeePayPalId(string payPalId)
        {
            return Payments.Where(x => x.Payee.PayPalId == payPalId);
        }

        public DateTimeOffset GetLastPaymentDate()
        {
           if (!PaymentAuditLogs.Any())
            {
                return new DateTimeOffset();
            }
            return PaymentAuditLogs.Max(x => x.DatePaid);
        }

        public IEnumerable<Uri> GetPartnerUris()
        {
            return Partners.Select(x => x.Site);
        }
    }
}
