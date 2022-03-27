using Newtonsoft.Json;

namespace SublessSignIn.Models
{
    public class StripeCheckoutViewModel
    {
        [JsonProperty("publishableKey")]
        public string PublishableKey { get; set; }
    }
}
