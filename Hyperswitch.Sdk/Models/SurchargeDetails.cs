using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class SurchargeInfo
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } // e.g., "fixed", "percentage"

        [JsonPropertyName("value")]
        public long? Value { get; set; } // Amount or percentage value
    }

    public class TaxOnSurchargeInfo
    {
        [JsonPropertyName("percentage")]
        public long? Percentage { get; set; } // Assuming this is a basis points or similar long value
    }

    public class SurchargeDetailsContainer
    {
        [JsonPropertyName("surcharge")]
        public SurchargeInfo? Surcharge { get; set; }

        [JsonPropertyName("tax_on_surcharge")]
        public TaxOnSurchargeInfo? TaxOnSurcharge { get; set; }

        [JsonPropertyName("display_surcharge_amount")]
        public long? DisplaySurchargeAmount { get; set; }

        [JsonPropertyName("display_tax_on_surcharge_amount")]
        public long? DisplayTaxOnSurchargeAmount { get; set; }

        [JsonPropertyName("display_total_surcharge_amount")]
        public long? DisplayTotalSurchargeAmount { get; set; }
    }
}
