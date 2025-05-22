using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class MerchantPMLRequest
    {
        [JsonPropertyName("client_secret")]
        public string? ClientSecret { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("amount")]
        public long? Amount { get; set; }

        [JsonPropertyName("profile_id")]
        public string? ProfileId { get; set; } 
    }
}
