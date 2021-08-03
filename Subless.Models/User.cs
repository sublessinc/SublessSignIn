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
        public ICollection<Creator> Creators { get; set; }
        [ForeignKey("Admin")]
        public ICollection<Partner> Partners { get; set; }
    }
}
