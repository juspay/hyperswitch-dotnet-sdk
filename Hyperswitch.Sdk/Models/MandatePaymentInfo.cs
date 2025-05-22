using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class SingleUseMandate
    {
        [JsonPropertyName("amount")]
        public long? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("start_date")]
        public string? StartDate { get; set; } // Consider DateTimeOffset if precise time zone handling is needed

        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }   // Consider DateTimeOffset

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class MandatePaymentInfo
    {
        [JsonPropertyName("single_use")]
        public SingleUseMandate? SingleUse { get; set; }
        
        // Add other mandate types like "multi_use" if they exist in the API
    }
}
