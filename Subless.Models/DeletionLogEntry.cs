using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class DeletionLogEntry
    {
        public string Role { get; set; }
        public DateTimeOffset DeleteDate { get; set; }
        public string Id { get; set; }
    }
}
