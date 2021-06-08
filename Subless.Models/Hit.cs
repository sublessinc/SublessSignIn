using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    [Index(nameof(UserId))]
    [Index(nameof(TimeStamp))]
    public class Hit
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public Uri Uri { get; set; }
        public DateTime TimeStamp { get; set; }
        public User User { get; set; }
    }
}
