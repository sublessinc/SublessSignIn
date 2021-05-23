using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class Creator
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public bool Active { get; set; }
        public Guid? ActivationCode { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? UserId { get; set; }
    }
}
