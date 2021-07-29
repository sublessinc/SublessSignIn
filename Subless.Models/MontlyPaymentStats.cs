using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class MontlyPaymentStats
    {
        public DateTime MonthStartDay { get;set; }
        public int DollarsPaid { get; set; }
        public int Payers { get; set; }
    }
}
