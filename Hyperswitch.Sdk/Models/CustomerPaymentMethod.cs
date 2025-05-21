using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class CustomerPaymentMethod
    {
        [JsonPropertyName("payment_token")]
        public string? PaymentToken { get; set; }

        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("payment_method_type")]
        public string? PaymentMethodType { get; set; }

        [JsonPropertyName("payment_method_issuer")]
        public string? PaymentMethodIssuer { get; set; }

        [JsonPropertyName("payment_method_issuer_code")]
        public string? PaymentMethodIssuerCode { get; set; }

        [JsonPropertyName("recurring_enabled")]
        public bool RecurringEnabled { get; set; }

        [JsonPropertyName("installment_payment_enabled")]
        public bool InstallmentPaymentEnabled { get; set; }

        [JsonPropertyName("payment_experience")]
        public List<string>? PaymentExperience { get; set; }

        [JsonPropertyName("card")]
        public CardSummary? Card { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        [JsonPropertyName("created")]
        public string? Created { get; set; } // ISO 8601 timestamp

        [JsonPropertyName("payment_method_data")]
        public object? PaymentMethodData { get; set; } 

        [JsonPropertyName("locker_id")]
        public string? LockerId { get; set; } 
    }
}
