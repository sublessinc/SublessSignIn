using Newtonsoft.Json;

namespace Subless.Models
{
    public class CreateCheckoutSessionRequest
    {
        [JsonProperty("priceId")]
        public int PriceId { get; set; }
    }
}