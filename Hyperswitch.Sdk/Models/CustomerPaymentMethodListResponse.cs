using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    /// <summary>
    /// Represents the response for listing a customer's payment methods.
    /// </summary>
    public class CustomerPaymentMethodListResponse
    {
        /// <summary>
        /// Gets or sets the ID of the customer.
        /// This might not be present in all response variants (e.g., when listing via client_secret).
        /// </summary>
        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the list of customer payment methods.
        /// The API returns this under "customer_payment_methods" when using client_secret.
        /// </summary>
        [JsonPropertyName("customer_payment_methods")] // Changed from "data"
        public List<CustomerPaymentMethod>? Data { get; set; }

        /// <summary>
        /// Gets or sets the count of payment methods returned.
        /// This might not be present in all response variants.
        /// </summary>
        [JsonPropertyName("payment_method_count")]
        public int? PaymentMethodCount { get; set; } // Made nullable

        /// <summary>
        /// Gets or sets a value indicating whether the customer is a guest.
        /// Present in the response when listing via client_secret.
        /// </summary>
        [JsonPropertyName("is_guest_customer")]
        public bool? IsGuestCustomer { get; set; } // Added field

        // Add other potential fields if the API supports more comprehensive pagination
        // e.g., "object": "list", "has_more": true, "url": "..."
    }
}
