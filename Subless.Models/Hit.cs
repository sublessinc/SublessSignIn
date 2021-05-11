using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class Hit
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Uri Uri { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
