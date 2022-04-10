using System;

namespace Subless.Models
{
    public class PartnerWriteModel
    {
        public Guid Id { get; set; }
        public string PayPalId { get; set; }
        #nullable enable
        public Uri? CreatorWebhook { get; set; }
        #nullable disable
    }
}
