using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SublessSignIn.Models
{
    public class StripeCheckoutViewModel
    {
        [JsonProperty("publishableKey")]
        public string PublishableKey { get; set; }

        [JsonProperty("basicPrice")]
        public string BasicPrice { get; set; }
    }
}
