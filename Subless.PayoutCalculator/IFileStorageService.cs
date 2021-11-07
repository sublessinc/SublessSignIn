using System.Collections.Generic;
using System.Threading.Tasks;

namespace Subless.Services
{
    public interface IFileStorageService
    {
        Task<bool> CanAccessS3();
        void WritePaymentsToCloudFileStore(Dictionary<string, double> masterPayoutList);
    }
}