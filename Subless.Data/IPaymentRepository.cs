using Subless.Models;
using System;
using System.Collections.Generic;

namespace Subless.Data
{
    public interface IPaymentRepository
    {
        DateTimeOffset GetLastPaymentDate();
        IEnumerable<Payment> GetPaymentsByPayeePayPalId(string payPalId);
        void LogDbStats();
        void SavePaymentAuditLogs(IEnumerable<PaymentAuditLog> logs);
        void SavePaymentLogs(IEnumerable<Payment> logs);
    }
}