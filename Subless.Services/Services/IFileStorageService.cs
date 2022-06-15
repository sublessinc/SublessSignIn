using System.Collections.Generic;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IFileStorageService
    {
        Task<bool> CanAccessS3();
        void WritePaymentsToCloudFileStore(List<PaymentAuditLog> masterPayoutList);
    }
}
