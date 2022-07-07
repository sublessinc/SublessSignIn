using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Services.Services
{
    public interface IPaymentLogsService
    {
        PaymentAuditLog GetLastPayment(Guid TargetId);
        DateTimeOffset GetLastPaymentDate();
        Tuple<DateTimeOffset, DateTimeOffset> GetLastPaymentPeriod();
        void SaveAuditLogs(IEnumerable<PaymentAuditLog> paymentAuditLogs);
        void SaveLogs(IEnumerable<Payment> paymentLogs);
    }
}
