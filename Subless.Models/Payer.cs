using System;

namespace Subless.Models
{
    public class Payer
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public double Payment { get; set; }
    }
}
