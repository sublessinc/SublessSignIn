using Subless.Models;
using System;
using System.Collections.Generic;

namespace Subless.Services
{
    public interface IPaymentLogsService
    {
        DateTimeOffset GetLastPaymentDate();
        void SaveAuditLogs(IEnumerable<PaymentAuditLog> paymentAuditLogs);
        void SaveLogs(IEnumerable<Payment> paymentLogs);
    }
}