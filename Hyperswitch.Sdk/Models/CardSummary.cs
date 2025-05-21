using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class CardSummary
    {
        [JsonPropertyName("last4_digits")]
        public string? Last4Digits { get; set; }

        [JsonPropertyName("expiry_month")]
        public string? ExpiryMonth { get; set; }

        [JsonPropertyName("expiry_year")]
        public string? ExpiryYear { get; set; }

        [JsonPropertyName("issuer_country_code")]
        public string? IssuerCountryCode { get; set; } 

        [JsonPropertyName("tokenization_status")]
        public string? TokenizationStatus { get; set; } 
    }
}
