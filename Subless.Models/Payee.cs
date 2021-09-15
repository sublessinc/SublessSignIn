using System;

namespace Subless.Models
{
    public class Payee
    {
        public Guid Id { get; set; }
        public double Payment { get; set; }
        public string PayPalId { get; set; }
    }
}
