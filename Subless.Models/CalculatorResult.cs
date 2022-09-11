using System;
using System.Collections.Generic;

namespace Subless.Models
{
    public class CalculatorResult
    {
        public CalculatorResult()
        {
            AllPayouts = new List<PaymentAuditLog>();
            PaymentsPerPayer = new Dictionary<string, List<Payment>>();
            IdleCustomerRollovers = new List<IdleCustomerRollover>();
        }
        public bool EmailSent { get; set; }
        public List<PaymentAuditLog> AllPayouts { get; set; }
        public Dictionary<string, List<Payment>> PaymentsPerPayer { get; set; }
        public List<IdleCustomerRollover> IdleCustomerRollovers { get; set; }
        public IEnumerable<PaymentAuditLog> UnvisitedCreators { get; set; }
    }
}
