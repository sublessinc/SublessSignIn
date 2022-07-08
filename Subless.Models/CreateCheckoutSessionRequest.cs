using Newtonsoft.Json;

namespace Subless.Models
{
    public class CreateCheckoutSessionRequest
    {
        [JsonProperty("priceId")]
        public int DesiredPrice { get; set; }
    }
}