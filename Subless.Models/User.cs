using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    [Index(nameof(CognitoId))]
    public class User
    {
        public Guid Id { get; set; }
        public string CognitoId { get; set; }
        public string StripeId { get; set; }
        public ICollection<Creator> Creators {get; set;}
    }
}
