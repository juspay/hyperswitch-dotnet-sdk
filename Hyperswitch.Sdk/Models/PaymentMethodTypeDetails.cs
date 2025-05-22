using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class PaymentExperience
    {
        [JsonPropertyName("payment_experience_type")]
        public string? PaymentExperienceType { get; set; } // e.g., "redirect_to_url"

        [JsonPropertyName("eligible_connectors")]
        public List<string>? EligibleConnectors { get; set; }
    }

    public class CardNetworkInfo
    {
        [JsonPropertyName("card_network")]
        public string? CardNetwork { get; set; } // e.g., "Visa"

        [JsonPropertyName("surcharge_details")]
        public SurchargeDetailsContainer? SurchargeDetails { get; set; }

        [JsonPropertyName("eligible_connectors")]
        public List<string>? EligibleConnectors { get; set; }
    }

    public class BankNameInfo
    {
        [JsonPropertyName("bank_name")]
        public List<string>? BankName { get; set; } // e.g., ["american_express"]

        [JsonPropertyName("eligible_connectors")]
        public List<string>? EligibleConnectors { get; set; }
    }

    public class BankDebitInfo
    {
        [JsonPropertyName("eligible_connectors")]
        public List<string>? EligibleConnectors { get; set; }
    }

    public class BankTransferInfo
    {
        [JsonPropertyName("eligible_connectors")]
        public List<string>? EligibleConnectors { get; set; }
    }
}
