using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class CustomerUpdateRequest
    {
        // Note: CustomerId is typically part of the URL for update, not the request body.

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("phone_country_code")]
        public string? PhoneCountryCode { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// The customer's address. Provide to update.
        /// If the API supports partial address updates, this structure is fine.
        /// If the entire address object must be replaced, ensure all sub-fields are set or cleared as intended.
        /// </summary>
        [JsonPropertyName("address")]
        public AddressDetails? Address { get; set; } // Uses the existing AddressDetails model

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        // Other fields that can be updated as per API documentation.
        // For example, default_payment_method_id if supported.
    }
}
