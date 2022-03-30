using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Subless.Models
{

    [Index(nameof(CognitoAppClientId), IsUnique = true)]
    public class Partner
    {
        public Guid Id { get; set; }
        public string CognitoAppClientId { get; set; }
        public string PayPalId { get; set; }
        public Uri Site { get; set; }
        public string UserPattern { get; set; }
        public ICollection<Creator> Creators { get; set; }
        public Guid Admin { get; set; }
#nullable enable
        public Uri? CreatorWebhook { get; set; }
    }
}
