using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class PaymentIntentResponse
    {
        [JsonPropertyName("payment_id")]
        public string? PaymentId { get; set; }

        [JsonPropertyName("merchant_id")]
        public string? MerchantId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; } // e.g., "requires_payment_method", "requires_confirmation", "succeeded", "failed"

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("amount_received")]
        public long? AmountReceived { get; set; }

        [JsonPropertyName("amount_capturable")]
        public long? AmountCapturable { get; set; }
        
        [JsonPropertyName("client_secret")]
        public string? ClientSecret { get; set; }

        [JsonPropertyName("created")]
        public string? Created { get; set; } // ISO 8601 DateTime string

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        
        [JsonPropertyName("phone")]
        public string? Phone {get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        [JsonPropertyName("next_action")]
        public NextAction? NextAction { get; set; }

        [JsonPropertyName("payment_method_id")]
        public string? PaymentMethodId { get; set; }
        
        [JsonPropertyName("shipping")]
        public Address? Shipping { get; set; } // Reusing Address model from PaymentIntentRequest

        [JsonPropertyName("billing")]
        public Address? Billing { get; set; } // Reusing Address model from PaymentIntentRequest

        [JsonPropertyName("return_url")]
        public string? ReturnUrl { get; set; }

        [JsonPropertyName("capture_method")]
        public string? CaptureMethod { get; set; }

        [JsonPropertyName("cancellation_reason")]
        public string? CancellationReason { get; set; }

        [JsonPropertyName("last_payment_error")]
        public PaymentError? LastPaymentError { get; set; }

        [JsonPropertyName("expires_on")]
        public string? ExpiresOn { get; set; } // ISO 8601 DateTime string

        [JsonPropertyName("updated")]
        public string? Updated { get; set; } // ISO 8601 DateTime string
        
        // Other fields like refunds, disputes, attempts can be added as needed.
    }

    public class NextAction
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } // e.g., "redirect_to_url", "use_stripe_sdk"

        [JsonPropertyName("redirect_to_url")]
        public string? RedirectToUrl { get; set; } // Changed from RedirectToUrl? to string?
        
        // Add other next action types as needed
    }

    // The RedirectToUrl class might be needed if other next_action types return a structured object.
    // For now, based on the provided response, it's a direct string.
    // public class RedirectToUrl 
    // {
    //     [JsonPropertyName("url")]
    //     public string? Url { get; set; }

    //     [JsonPropertyName("return_url")]
    //     public string? ReturnUrl { get; set; }
    // }

    public class PaymentError
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("decline_code")]
        public string? DeclineCode { get; set; }

        [JsonPropertyName("param")]
        public string? Param { get; set; }

        [JsonPropertyName("payment_method_id")]
        public string? PaymentMethodId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; } // e.g., "card_error", "invalid_request_error"
    }
}
