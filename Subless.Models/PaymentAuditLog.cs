using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    
    public class PaymentAuditLog
    {
        public Guid Id { get; set; }
        public string PayoneerId { get; set; }
        public double Payment { get; set; }
        public DateTime DatePaid { get; set; }
    }
}
