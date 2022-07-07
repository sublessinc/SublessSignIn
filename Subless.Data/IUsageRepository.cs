using Subless.Models;

namespace Subless.Data
{
    public interface IUsageRepository
    {
        void SaveUsageStat(UsageStat stat);
    }
}
