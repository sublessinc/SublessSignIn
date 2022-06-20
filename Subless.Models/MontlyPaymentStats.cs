using System;

namespace Subless.Models
{
    public class MontlyPaymentStats
    {
        public DateTimeOffset MonthStart { get; set; }
        public DateTimeOffset MonthEnd { get; set; }
        public double Revenue { get; set; }
        public double PaymentProcessorFees { get; set; }

        public double Payment { get; set; }
    }
}
