using System;

namespace Subless.Models
{
    public class CreatorHitCount
    {
        public string CreatorName { get; set; }
        public int Hits { get; set; }
        public Guid CreatorId { get; set; }
    }
}
