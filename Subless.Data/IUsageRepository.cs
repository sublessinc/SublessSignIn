using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Data
{
    public interface IUsageRepository
    {
        void SaveUsageStat(UsageStat stat);
    }
}
