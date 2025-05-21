using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class RefundListResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("data")]
        public List<RefundResponse>? Data { get; set; }

        // Other potential fields like 'has_more', 'url', 'object' (e.g. "list")
        // can be added if the API provides them.
    }
}
