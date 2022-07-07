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

        public Tuple<DateTimeOffset, DateTimeOffset> GetLastPaymentPeriod()
        {
            if (!PaymentAuditLogs.Any())
            {
                return null;
            }
            var lastLog = PaymentAuditLogs.OrderByDescending(x => x.PaymentPeriodEnd).FirstOrDefault();
            return new Tuple<DateTimeOffset, DateTimeOffset> (lastLog.PaymentPeriodStart, lastLog.PaymentPeriodEnd);
        }

        public PaymentAuditLog GetLastPayment(Guid TargetId)
        {
            if (!PaymentAuditLogs.Any(x => x.TargetId == TargetId))
            {
                return null;
            }
            return PaymentAuditLogs.Where(x => x.TargetId == TargetId).OrderByDescending(x => x.PaymentPeriodEnd).FirstOrDefault();
        }

        public IEnumerable<PaymentAuditLog> GetAllPaymentsToUser(Guid targetId)
        {          
            return PaymentAuditLogs.Where(x => x.TargetId == targetId).OrderByDescending(x => x.PaymentPeriodEnd);
        }

        public void SavePaymentLogs(IEnumerable<Payment> logs)
        {
            Payments.AddRange(logs);
            SaveChanges();
        }
    }
}
