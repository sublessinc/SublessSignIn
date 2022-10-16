using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    [Index(nameof(CreatorId), IsUnique = false)]
    public class CreatorMessage
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
