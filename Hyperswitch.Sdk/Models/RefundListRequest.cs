using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class AmountFilter // Defined here if not already in a shared location
    {
        [JsonPropertyName("start_amount")]
        public long? StartAmount { get; set; }

        [JsonPropertyName("end_amount")]
        public long? EndAmount { get; set; }
    }

    public class RefundListRequest
    {
        [JsonPropertyName("payment_id")]
        public string? PaymentId { get; set; }

        [JsonPropertyName("refund_id")]
        public string? RefundId { get; set; } 

        [JsonPropertyName("profile_id")]
        public string? ProfileId { get; set; }

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("offset")]
        public int? Offset { get; set; } 

        [JsonPropertyName("start_time")] 
        public string? StartTime { get; set; } // ISO 8601 UTC string

        [JsonPropertyName("end_time")]
        public string? EndTime { get; set; } // ISO 8601 UTC string

        [JsonPropertyName("amount_filter")]
        public AmountFilter? AmountFilter { get; set; }

        [JsonPropertyName("connector")]
        public List<string>? Connector { get; set; }

        [JsonPropertyName("merchant_connector_id")]
        public List<string>? MerchantConnectorId { get; set; }

        [JsonPropertyName("currency")]
        public List<string>? Currency { get; set; } 

        [JsonPropertyName("refund_status")]
        public List<string>? RefundStatus { get; set; } 
    }
}
