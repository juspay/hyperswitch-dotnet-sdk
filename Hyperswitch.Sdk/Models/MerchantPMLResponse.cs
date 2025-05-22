using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    /// <summary>
    /// Represents the response from the API call to list available merchant payment method configurations.
    /// This structure is based on the JSON example provided by the user.
    /// </summary>
    public class MerchantPMLResponse
    {
        [JsonPropertyName("redirect_url")]
        public string? RedirectUrl { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("payment_methods")]
        public List<PaymentMethodGroup>? PaymentMethods { get; set; }

        [JsonPropertyName("mandate_payment")]
        public MandatePaymentInfo? MandatePayment { get; set; }

        [JsonPropertyName("merchant_name")]
        public string? MerchantName { get; set; }

        [JsonPropertyName("show_surcharge_breakup_screen")]
        public bool? ShowSurchargeBreakupScreen { get; set; }

        [JsonPropertyName("payment_type")]
        public string? PaymentType { get; set; } // e.g., "normal"

        [JsonPropertyName("request_external_three_ds_authentication")]
        public bool? RequestExternalThreeDsAuthentication { get; set; }

        [JsonPropertyName("collect_shipping_details_from_wallets")]
        public bool? CollectShippingDetailsFromWallets { get; set; }

        [JsonPropertyName("collect_billing_details_from_wallets")]
        public bool? CollectBillingDetailsFromWallets { get; set; }

        [JsonPropertyName("is_tax_calculation_enabled")]
        public bool? IsTaxCalculationEnabled { get; set; }
    }
}
