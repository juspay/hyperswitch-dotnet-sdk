using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json; // Required for JsonElement

namespace Hyperswitch.Sdk.Models
{
    public class DetailedPaymentMethodType
    {
        [JsonPropertyName("payment_method_type")]
        public string? PaymentMethodType { get; set; } // e.g., "ach", "credit"

        [JsonPropertyName("payment_experience")]
        public List<PaymentExperience>? PaymentExperience { get; set; }

        [JsonPropertyName("card_networks")]
        public List<CardNetworkInfo>? CardNetworks { get; set; }

        [JsonPropertyName("bank_names")]
        public List<BankNameInfo>? BankNames { get; set; }

        [JsonPropertyName("bank_debits")]
        public BankDebitInfo? BankDebits { get; set; } // Assuming single object based on JSON

        [JsonPropertyName("bank_transfers")]
        public BankTransferInfo? BankTransfers { get; set; } // Assuming single object

        // For "required_fields", using JsonElement as its structure can be dynamic (key-value pairs)
        // or a specific class if the structure is fixed and known.
        [JsonPropertyName("required_fields")]
        public JsonElement? RequiredFields { get; set; } 

        [JsonPropertyName("surcharge_details")]
        public SurchargeDetailsContainer? SurchargeDetails { get; set; }

        [JsonPropertyName("pm_auth_connector")]
        public string? PmAuthConnector { get; set; }
    }
}
