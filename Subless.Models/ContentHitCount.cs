using System;

namespace Subless.Models
{
    public class ContentHitCount
    {
        public Uri Content { get; set; }
        public string Title { get; set; }
        public int Hits { get; set; }
        public Uri Favicon {get;set;}
        public Guid PartnerId { get; set; }
    }
}
