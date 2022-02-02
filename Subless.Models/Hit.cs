using System;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    [Index(nameof(CognitoId), IsUnique = false)]
    [Index(nameof(TimeStamp))]
    public class Hit
    {
        public Guid Id { get; set; }

        public string CognitoId { get; set; }
        public Guid CreatorId { get; set; }
        public Guid PartnerId { get; set; }
        public Uri Uri { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public User User { get; set; }
    }
}
