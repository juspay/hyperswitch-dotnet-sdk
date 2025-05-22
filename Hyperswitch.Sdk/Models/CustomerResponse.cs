using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class CustomerResponse
    {
        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

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
        /// The customer's address.
        /// </summary>
        [JsonPropertyName("address")]
        public AddressDetails? Address { get; set; } // Uses the existing AddressDetails model

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Timestamp of when the customer object was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; } // Assuming ISO 8601 string

        // The Hyperswitch_Customer_SDK README example did not show default_payment_method_id,
        // but it's a common field. Add if confirmed by API docs.
        // [JsonPropertyName("default_payment_method_id")]
        // public string? DefaultPaymentMethodId { get; set; } 
    }
}
