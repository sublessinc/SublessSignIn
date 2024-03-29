using System;

namespace Subless.Models
{
    public class PartnerStats
    {
        public int Views { get; set; }
        public int Visitors { get; set; }
        public int Creators { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
        public DateTimeOffset PeriodEnd { get; set; }
    }
}
