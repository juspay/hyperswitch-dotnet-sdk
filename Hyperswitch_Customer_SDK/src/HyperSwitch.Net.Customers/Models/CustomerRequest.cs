using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace HyperSwitch.Net.Customers.Models
{
    public class CustomerRequest
    {
        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

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

        [JsonPropertyName("address")]
        public AddressDetails? Address { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }
} 