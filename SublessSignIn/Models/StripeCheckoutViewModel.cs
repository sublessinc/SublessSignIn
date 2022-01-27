using Newtonsoft.Json;
using System.Collections.Generic;

namespace SublessSignIn.Models
{
    public class StripeCheckoutViewModel
    {
        [JsonProperty("publishableKey")]
        public string PublishableKey { get; set; }
    }
}
