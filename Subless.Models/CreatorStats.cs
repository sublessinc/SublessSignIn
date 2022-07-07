using System;

namespace Subless.Models
{
    public class CreatorStats
    {
        public int Views { get; set; }
        public int Visitors { get; set; }
        public int PiecesOfContent { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
        public DateTimeOffset PeriodEnd { get; set; }
    }
}
