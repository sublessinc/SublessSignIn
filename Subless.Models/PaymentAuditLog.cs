using System;

namespace Subless.Models
{

    public class PaymentAuditLog
    {
        public Guid Id { get; set; }
        public string PayPalId { get; set; }
        public Guid TargetId { get; set; }
        public PayeeType PayeeType { get; set; }
        public double Payment { get; set; }
        public double Revenue { get; set; }
        public double Fees { get; set; }
        public bool Paid { get; set; }
        public DateTimeOffset DatePaid { get; set; }
    }
}
