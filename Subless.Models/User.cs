using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    [Index(nameof(CognitoId), IsUnique = true)]
    public class User
    {
        public Guid Id { get; set; }
        public bool IsAdmin { get; set; }
        public string CognitoId { get; set; }
        public string StripeSessionId { get; set; }
        public string StripeCustomerId { get; set; }
        public DateTimeOffset? Replica_SubcriptionDate { get; set; }
        public bool Replica_IsPaying { get; set; }
        public string Replica_Email { get; set; }
        public long? Replica_Subscription { get; set; }
        public bool AcceptedTerms { get; set; }
        public bool WelcomeEmailSent { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public ICollection<Creator> Creators { get; set; }
        [ForeignKey("Admin")]
        public ICollection<Partner> Partners { get; set; }
    }
}
