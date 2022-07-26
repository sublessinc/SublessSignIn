using System;

namespace Subless.Models
{
    public class CalculatorExecution
    {
        public Guid Id { get; set; }
        public DateTimeOffset PeriodStart { get; set; }
        public DateTimeOffset PeriodEnd { get; set; }
        public DateTimeOffset DateQueued { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset DateExecuted { get; set; }
        public string Result { get; set; }

    }
}
