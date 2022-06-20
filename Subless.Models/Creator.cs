using System;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    [Index(nameof(Username), nameof(PartnerId), IsUnique = true)]
    [Index(nameof(ActivationCode))]
    public class Creator
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public bool Active { get; set; }
        public Guid? ActivationCode { get; set; }
        public DateTimeOffset ActivationExpiration { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? UserId { get; set; }
        public string PayPalId { get; set; }
        public string Email { get; set; }
        public bool AcceptedTerms { get; set; }
        public DateTimeOffset CreateDate { get; set; }

    }
}
