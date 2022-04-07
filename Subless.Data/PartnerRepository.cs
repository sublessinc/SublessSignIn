using Microsoft.EntityFrameworkCore;
using Subless.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Subless.Data
{
    public partial class Repository : DbContext, IPartnerRepository
    {
        internal DbSet<Partner> Partners { get; set; }

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
            return Partners.FirstOrDefault(partner => partner.Sites.Any(x=> x == uri));
        }

        public IEnumerable<Uri> GetPartnerUris()
        {
            List<Uri> uris = new List<Uri>();
            foreach (var partner in Partners)
            {
                foreach (var uri in partner.Sites)
                {
                    uris.Add(uri);
                }
            }
            return uris;
        }


        public Creator GetCreatorByPartnerAndUsername(string partnerCognitoId, string username)
        {
            var partners = Partners.Include(p => p.Creators).Where(x => x.CognitoAppClientId == partnerCognitoId);
            var partner = partners.Single();
            var creator = partner.Creators.FirstOrDefault(y => y.Username == username);
            return creator;
        }
    }
}
