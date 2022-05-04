using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Subless.Models;

namespace Subless.Data
{
    public partial class Repository : DbContext, IUsageRepository
    {
        internal DbSet<UsageStat> UsageStats{ get; set; }

        public void SaveUsageStat(UsageStat stat)
        {
            UsageStats.Add(stat);
            SaveChanges();
        }
    }
}
