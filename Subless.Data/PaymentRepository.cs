using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Subless.Models;

namespace Subless.Data
{
    public partial class Repository : DbContext, IPaymentRepository
    {

        internal DbSet<Payment> Payments { get; set; }
        internal DbSet<PaymentAuditLog> PaymentAuditLogs { get; set; }

        public void SavePaymentAuditLogs(IEnumerable<PaymentAuditLog> logs)
        {
            PaymentAuditLogs.AddRange(logs);
            SaveChanges();
        }

        public IEnumerable<Payment> GetPaymentsByPayeePayPalId(string payPalId)
        {
            return Payments.Where(x => x.Payee.PayPalId == payPalId);
        }

        public DateTimeOffset GetLastPaymentDate()
        {
            if (!PaymentAuditLogs.Any())
            {
                return new DateTimeOffset();
            }
            return PaymentAuditLogs.Max(x => x.DatePaid);
        }

        public void SavePaymentLogs(IEnumerable<Payment> logs)
        {
            Payments.AddRange(logs);
            SaveChanges();
        }
    }
}
