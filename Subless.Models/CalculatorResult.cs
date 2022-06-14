﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Models
{
    public class CalculatorResult
    {
        public CalculatorResult()
        {
            AllPayouts = new List<PaymentAuditLog>();
            PaymentsPerPayer = new Dictionary<string, List<Payment>>();
            IdleCustomerStripeIds = new List<string>();
        }
        public bool EmailSent { get; set; }
        public List<PaymentAuditLog> AllPayouts { get; set; }
        public Dictionary<string, List<Payment>> PaymentsPerPayer {get;set;}

        public List<string> IdleCustomerStripeIds { get; set; }

    }
}
