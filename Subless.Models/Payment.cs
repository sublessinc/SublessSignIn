using System;

namespace Subless.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Payee Payee { get; set; }
        public Payer Payer { get; set; }
        public double Amount { get; set; }
        public DateTime DateSent { get; set; }
    }

}
