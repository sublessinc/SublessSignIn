using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class SubscriptionStatus
    {
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public DateTimeOffset? BillingDate {get;set;}
        public DateTimeOffset? CancellationDate { get; set; }
        public DateTimeOffset? SubscriptionEndDate { get; set; }
    }
}
