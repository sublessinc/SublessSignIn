using System;

namespace Subless.Models
{
    public class HitView: IFaviconable
    {
        public Uri Content { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; }
        public Uri Favicon { get; set; }
        public Guid PartnerId { get; set; }
    }
}
