using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    /// <summary>
    /// Represents the response object for a payout intent.
    /// </summary>
    public class PayoutIntentResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the payout.
        /// </summary>
        [JsonPropertyName("payout_id")]
        public string? PayoutId { get; set; }

        /// <summary>
        /// Gets or sets the merchant ID associated with this payout.
        /// </summary>
        [JsonPropertyName("merchant_id")]
        public string? MerchantId { get; set; }

        /// <summary>
        /// Gets or sets the status of the payout (e.g., "success", "pending", "failed").
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the amount of the payout in the smallest currency unit.
        /// </summary>
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// Gets or sets the three-letter ISO currency code.
        /// </summary>
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of when the payout intent was created (ISO 8601 format).
        /// </summary>
        [JsonPropertyName("created")]
        public string? Created { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last update to the payout intent (ISO 8601 format).
        /// </summary>
        [JsonPropertyName("updated")]
        public string? Updated { get; set; }

        /// <summary>
        /// Gets or sets the description of the payout.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the ID of the customer associated with this payout.
        /// </summary>
        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the payout.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number associated with the payout.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets metadata associated with the payout intent.
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        /// <summary>
        /// Gets or sets the billing address associated with the payout.
        /// </summary>
        [JsonPropertyName("billing")]
        public PayoutAddress? Billing { get; set; }

        /// <summary>
        /// Gets or sets the payout method ID used for this payout.
        /// </summary>
        [JsonPropertyName("payout_method_id")]
        public string? PayoutMethodId { get; set; }

        /// <summary>
        /// Gets or sets the type of payout method (e.g., "sepa", "ach").
        /// </summary>
        [JsonPropertyName("payout_method_type")]
        public string? PayoutMethodType { get; set; }

        /// <summary>
        /// Gets or sets details of the last error that occurred during the payout, if any.
        /// </summary>
        [JsonPropertyName("last_payout_error")]
        public PayoutError? LastPayoutError { get; set; }

        /// <summary>
        /// Gets or sets the reason for cancellation, if the payout was cancelled.
        /// </summary>
        [JsonPropertyName("cancellation_reason")]
        public string? CancellationReason { get; set; }
    }

    /// <summary>
    /// Represents an error that occurred during payout processing.
    /// </summary>
    public class PayoutError
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the human-readable error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the parameter related to the error, if applicable.
        /// </summary>
        [JsonPropertyName("param")]
        public string? Param { get; set; }

        /// <summary>
        /// Gets or sets the ID of the payout method associated with the error, if applicable.
        /// </summary>
        [JsonPropertyName("payout_method_id")]
        public string? PayoutMethodId { get; set; }

        /// <summary>
        /// Gets or sets the type of error (e.g., "bank_error", "invalid_request_error").
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
