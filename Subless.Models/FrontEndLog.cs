using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class FrontEndLog
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int Level { get; set; } 
    }
}
