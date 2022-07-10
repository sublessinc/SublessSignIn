using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class PaymentExecution
    {
        public Guid Id { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
        public DateTimeOffset PeriodEnd { get; set; }
        public DateTimeOffset DateQueued { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset DateExecuted { get; set; }
    }
}
