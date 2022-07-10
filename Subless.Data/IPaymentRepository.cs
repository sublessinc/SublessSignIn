using System;
using System.Collections.Generic;
using Subless.Models;

namespace Subless.Data
{
    public interface IPaymentRepository
    {
        IEnumerable<PaymentAuditLog> GetAllPaymentsToUser(Guid targetId);
        PaymentAuditLog GetLastPayment(Guid TargetId);
        DateTimeOffset GetLastPaymentDate();
        Tuple<DateTimeOffset, DateTimeOffset> GetLastPaymentPeriod();
        IEnumerable<Payment> GetPaymentsByPayeePayPalId(string payPalId);
        void LogDbStats();
        void SavePaymentAuditLogs(IEnumerable<PaymentAuditLog> logs);
        void SavePaymentLogs(IEnumerable<Payment> logs);
    }
}
