using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    /// <summary>
    /// Represents the request parameters for confirming a payment.
    /// </summary>
    public class PaymentConfirmRequest
    {
        /// <summary>
        /// The type of payment method being confirmed, e.g., "card".
        /// Required if payment_method_data is provided.
        /// </summary>
        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Payment method data, if not already associated with the PaymentIntent
        /// or if you need to update/confirm with specific details (for new payment methods).
        /// </summary>
        [JsonPropertyName("payment_method_data")]
        public PaymentMethodData? PaymentMethodData { get; set; }

        /// <summary>
        /// A token representing a saved payment method. 
        /// Use this if confirming with an existing payment method for the customer.
        /// </summary>
        [JsonPropertyName("payment_token")] // Changed from payment_method_token to match cURL
        public string? PaymentMethodToken { get; set; }

        /// <summary>
        /// Gets or sets the CVC/CVV code for the card, required when confirming with a token.
        /// </summary>
        [JsonPropertyName("card_cvc")]
        public string? CardCvc { get; set; } // Added top-level CardCvc

        /// <summary>
        /// The URL to redirect your customer back to after they authenticate on the payment processor's page.
        /// This is often required if the confirmation triggers a 3DS flow or other redirection.
        /// </summary>
        [JsonPropertyName("return_url")]
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Shipping details, if updated or provided at the point of confirmation.
        /// </summary>
        [JsonPropertyName("shipping")]
        public AddressDetails? Shipping { get; set; }

        /// <summary>
        /// Billing details, if updated or provided at the point of confirmation.
        /// </summary>
        [JsonPropertyName("billing")]
        public AddressDetails? Billing { get; set; }
        
        /// <summary>
        /// Specifies how the payment should be captured.
        /// Can be set or overridden at the confirm step.
        /// Possible values: "automatic", "manual".
        /// </summary>
        [JsonPropertyName("capture_method")]
        public string? CaptureMethod { get; set; }

        /// <summary>
        /// Browser information, potentially required at confirm step.
        /// </summary>
        [JsonPropertyName("browser_info")]
        public BrowserInfo? BrowserInfo { get; set; }

        /// <summary>
        /// Gets or sets customer acceptance details.
        /// This is required by some payment flows or for saving payment methods.
        /// </summary>
        [JsonPropertyName("customer_acceptance")]
        public CustomerAcceptance? CustomerAcceptance { get; set; }
        
        // Note: ClientSecret was removed from this request model as it caused API errors.
        // AuthenticationType is also typically set at create, not confirm, unless API specifies otherwise.
    }
}
