using System;

namespace Subless.Models
{
    public class Payee
    {
        public Payee()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Payment { get; set; }
        public string PayPalId { get; set; }
    }
}
