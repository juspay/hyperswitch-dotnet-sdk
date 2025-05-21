using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace HyperSwitch.Net.Customers.Models
{
    public class CustomerResponse
    {
        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; } = null!;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("phone_country_code")]
        public string? PhoneCountryCode { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("created_at")]
        public System.DateTime CreatedAt { get; set; }

        [JsonPropertyName("address")]
        public AddressDetails? Address { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
} 