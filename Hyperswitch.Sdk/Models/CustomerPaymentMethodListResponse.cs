using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class CustomerPaymentMethodListResponse
    {
        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

        [JsonPropertyName("data")]
        public List<CustomerPaymentMethod>? Data { get; set; }

        [JsonPropertyName("payment_method_count")]
        public int PaymentMethodCount { get; set; }

        // Add other potential fields if the API supports more comprehensive pagination
        // e.g., "object": "list", "has_more": true, "url": "..."
    }
}
