using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class IdleCustomerRollover
    {
        public string CognitoId { get; set; }
        public string CustomerId { get; set; }
        public double Payment { get; set; }
    }
}
