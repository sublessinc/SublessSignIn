using System;

namespace Subless.Models
{

    public class PaymentAuditLog
    {
        public Guid Id { get; set; }
        public string PayPalId { get; set; }
        public double Payment { get; set; }
        public DateTime DatePaid { get; set; }
    }
}
