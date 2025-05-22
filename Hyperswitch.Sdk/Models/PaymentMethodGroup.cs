using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class PaymentMethodGroup
    {
        /// <summary>
        /// The broad category of the payment method, e.g., "card", "wallet", "bank_transfer".
        /// </summary>
        [JsonPropertyName("payment_method")]
        public string? PaymentMethodCategory { get; set; }

        /// <summary>
        /// A list of specific payment method types and their configurations within this category.
        /// </summary>
        [JsonPropertyName("payment_method_types")]
        public List<DetailedPaymentMethodType>? PaymentMethodTypes { get; set; }
    }
}
