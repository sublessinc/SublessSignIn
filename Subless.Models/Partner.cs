using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    [Index(nameof(CognitoAppClientId))]
    public class Partner
    {
        public Guid Id { get; set; }
        public string CognitoAppClientId { get; set; }
        public Uri Site { get; set; }
        public string UserPattern { get; set; }
        public ICollection<Creator> Creators { get; set; }
    }
}
