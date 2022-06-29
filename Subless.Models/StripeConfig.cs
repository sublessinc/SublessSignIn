namespace Subless.Models
{
    public class StripeConfig
    {
        public string PublishableKey { get; set; }
        public string SecretKey { get; set; }
        public string WebhookSecret { get; set; }

        public string Domain { get; set; }
        public string SublessPayPalId { get; set; }
        public int ParallelRequests { get; set; }
    }
}
