using System.Collections.Generic;

namespace Subless.Services
{
    public interface IFileStorageService
    {
        void WritePaymentsToCloudFileStore(Dictionary<string, double> masterPayoutList);
    }
}