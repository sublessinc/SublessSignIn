﻿using System;

namespace Subless.Models
{
    public class PartnerWriteModel
    {
        public Guid Id { get; set; }
        public string PayPalId { get; set; }
        public Uri? CreatorWebhook { get; set; }
    }
}
