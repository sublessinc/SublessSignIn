using System.Collections.Generic;
namespace Subless.Models
{
    public class IdleCustomerRollover
    {
        public string CognitoId { get; set; }
        public string CustomerId { get; set; }
        public double Payment { get; set; }
        public IEnumerable<Hit> PreviousHits { get; set; }
    }
}
