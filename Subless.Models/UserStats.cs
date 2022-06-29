using System;

namespace Subless.Models
{
    public class UserStats
    {
        public int Views { get; set; }
        public int Creators { get; set; }
        public int Partners { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
        public DateTimeOffset PeriodEnd { get; set; }
    }
}
