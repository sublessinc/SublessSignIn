﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Subless.Models
{
    [Index(nameof(Username))]
    [Index(nameof(ActivationCode))]
    public class Creator
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public bool Active { get; set; }
        public Guid? ActivationCode { get; set; }
        public DateTime ActivationExpiration { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? UserId { get; set; }
    }
}