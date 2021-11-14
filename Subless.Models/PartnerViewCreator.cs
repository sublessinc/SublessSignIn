using System;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    public class PartnerViewCreator
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public bool Active { get; set; }
        public Guid PartnerId { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
    }
}
