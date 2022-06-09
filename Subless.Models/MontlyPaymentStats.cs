using System;

namespace Subless.Models
{
    public class MontlyPaymentStats
    {
        public DateTimeOffset MonthStartDay { get; set; }
        public double DollarsPaid { get; set; }
        public int Payers { get; set; }
    }
}
