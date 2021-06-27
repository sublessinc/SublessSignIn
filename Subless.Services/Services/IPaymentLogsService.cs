using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services
{
    public interface IPaymentLogsService
    {
        void SaveAuditLogs(IEnumerable<PaymentAuditLog> paymentAuditLogs);
        void SaveLogs(IEnumerable<Payment> paymentLogs);
    }
}