using System.Text.Json.Serialization;

namespace HyperSwitch.Net.Customers.Models
{
    public class CustomerDeleteResponse
    {
        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; } = null!;

        [JsonPropertyName("customer_deleted")]
        public bool CustomerDeleted { get; set; }
    }
} 