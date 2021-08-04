using System;

namespace Subless.Models
{
    public class MontlyPaymentStats
    {
        public DateTime MonthStartDay { get; set; }
        public int DollarsPaid { get; set; }
        public int Payers { get; set; }
    }
}
