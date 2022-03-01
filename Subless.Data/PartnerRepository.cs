using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Data
{
    public class PartnerRepository : DbContext
    {
        private readonly IOptions<DatabaseSettings> _options;
        internal DbSet<Partner> Partners{ get; set; }
        public void LogDbStats()
        {
            logger.LogWarning($"User count: {Users.Count()}");
            logger.LogWarning($"Hits count: {Hits.Count()}");
            logger.LogWarning($"Partners count: {Partners.Count()}");
            logger.LogWarning($"Creators count: {Creators.Count()}");
            logger.LogWarning($"Payments count: {Payments.Count()}");
            logger.LogWarning($"PaymentAuditLogs count: {PaymentAuditLogs.Count()}");
        }
        public void DeletePartner(Partner partner)
        {
            Partners.Remove(partner);
            SaveChanges();
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
            return Partners.Include(p => p.Creators).ToList();
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

        public Partner GetPartnerByUri(Uri uri)
        {
            return Partners.FirstOrDefault(partner => partner.Site == uri);
        }

        public IEnumerable<Uri> GetPartnerUris()
        {
            return Partners.Select(x => x.Site);
        }
    }
}
