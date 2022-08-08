using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class CreatorHitCount
    {
        public string CreatorName { get; set; }
        public int Hits { get; set; }
        public Guid CreatorId { get; set; }
    }
}
