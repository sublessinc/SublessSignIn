using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IPaymentLogsService
    {
        DateTimeOffset GetLastPaymentDate();
        void SaveAuditLogs(IEnumerable<PaymentAuditLog> paymentAuditLogs);
        void SaveLogs(IEnumerable<Payment> paymentLogs);
    }
}