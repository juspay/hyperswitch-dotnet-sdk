using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    /// <summary>
    /// Request parameters for listing available merchant payment method configurations.
    /// These would typically be sent as query parameters.
    /// </summary>
    public class MerchantPMLRequest
    {
        /// <summary>
        /// Optional: Filter payment methods by country (e.g., "US", "IN").
        /// </summary>
        [JsonPropertyName("country")] // Assuming query param name
        public string? Country { get; set; }

        /// <summary>
        /// Optional: Filter payment methods by currency (e.g., "USD", "EUR").
        /// </summary>
        [JsonPropertyName("currency")] // Assuming query param name
        public string? Currency { get; set; }

        /// <summary>
        /// Optional: Filter payment methods by amount (in smallest currency unit).
        /// Some payment methods are only available for certain amount ranges.
        /// </summary>
        [JsonPropertyName("amount")] // Assuming query param name
        public long? Amount { get; set; }

        /// <summary>
        /// Optional: The ID of the merchant profile for which to list payment methods.
        /// If not provided, might default to the primary profile associated with the API key.
        /// </summary>
        [JsonPropertyName("profile_id")] // Assuming query param name
        public string? ProfileId { get; set; }

        // Add other potential filters like 'locale', 'customer_id' if supported by the API.
    }
}
