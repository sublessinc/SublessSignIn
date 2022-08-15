using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class StripeSync
    {       
        public Guid Id { get; set; }
        public DateTimeOffset DateQueued { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsCompleted { get; set; }
    }
}
