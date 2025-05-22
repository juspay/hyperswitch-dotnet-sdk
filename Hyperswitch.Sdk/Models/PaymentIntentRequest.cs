using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class PaymentIntentRequest
    {
        /// <summary>
        /// The amount to be charged, in the smallest currency unit (e.g., cents for USD).
        /// </summary>
        [JsonPropertyName("amount")]
        public long? Amount { get; set; } // Made nullable to align with some optional scenarios, though typically required.

        /// <summary>
        /// Three-letter ISO currency code.
        /// </summary>
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("profile_id")]
        public string? ProfileId { get; set; }

        /// <summary>
        /// The ID of the customer to whom this payment belongs.
        /// </summary>
        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }

        /// <summary>
        /// A description for the payment.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        /// <summary>
        /// Payer's full name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Email address of the customer.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        
        /// <summary>
        /// Phone number of the customer.
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone {get; set; }

        [JsonPropertyName("phone_country_code")]
        public string? PhoneCountryCode { get; set; }

        /// <summary>
        /// Specifies whether the payment should be immediately confirmed.
        /// Default is true. If false, the payment intent will be created in a 'requires_confirmation' state.
        /// </summary>
        [JsonPropertyName("confirm")]
        public bool? Confirm { get; set; } = true;

        /// <summary>
        /// Describes how the payment should be captured.
        /// Possible values: "automatic" (default), "manual".
        /// </summary>
        [JsonPropertyName("capture_method")]
        public string? CaptureMethod { get; set; } // "automatic", "manual"

        [JsonPropertyName("capture_on")]
        public string? CaptureOn { get; set; } // ISO 8601 DateTime string e.g. "2029-09-10T10:11:12Z"

        [JsonPropertyName("amount_to_capture")]
        public long? AmountToCapture { get; set; }
        
        /// <summary>
        /// The URL to redirect your customer back to after they authenticate on the payment processor's page.
        /// </summary>
        [JsonPropertyName("return_url")]
        public string? ReturnUrl { get; set; }

        [JsonPropertyName("authentication_type")]
        public string? AuthenticationType { get; set; } // e.g. "no_three_ds"

        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; } // e.g. "card"

        [JsonPropertyName("payment_method_type")]
        public string? PaymentMethodType { get; set; } // e.g. "credit"

        /// <summary>
        /// Payment method data.
        /// </summary>
        [JsonPropertyName("payment_method_data")]
        public PaymentMethodData? PaymentMethodData { get; set; }
        
        /// <summary>
        /// Shipping address for the order.
        /// </summary>
        [JsonPropertyName("shipping")]
        public Address? Shipping { get; set; }

        /// <summary>
        /// Billing address for the order. (Note: cURL shows billing inside payment_method_data for card, this is top-level billing)
        /// If card billing is different, it should be inside PaymentMethodData.Card.Billing
        /// </summary>
        [JsonPropertyName("billing")]
        public Address? Billing { get; set; }

        [JsonPropertyName("browser_info")]
        public BrowserInfo? BrowserInfo { get; set; }

        /// <summary>
        /// Set of key-value pairs that you can attach to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        [JsonPropertyName("order_details")]
        public List<OrderDetailItem>? OrderDetails { get; set; }
        
        [JsonPropertyName("payment_link")]
        public bool? PaymentLink { get; set; }

        /// <summary>
        /// Indicates if this payment method should be saved for future use by the customer.
        /// Common values: "on_session", "off_session".
        /// Requires customer_id to be set.
        /// </summary>
        [JsonPropertyName("setup_future_usage")]
        public string? SetupFutureUsage { get; set; }


        // Add other fields as per Hyperswitch documentation:
        // e.g. statement_descriptor, statement_descriptor_suffix etc.
    }

    public class PaymentMethodData
    {
        /// <summary>
        /// Type of the payment method (e.g., "card", "paypal").
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; } // Not present in cURL for card, but generally good for SDK structure

        [JsonPropertyName("card")]
        public CardDetails? Card { get; set; }

        /// <summary>
        /// Billing address specific to this payment method.
        /// As per cURL, this is nested under payment_method_data for card payments.
        /// </summary>
        [JsonPropertyName("billing")]
        public Address? Billing { get; set; } 
        
        // Add other payment method types like "paypal", "klarna" etc. as needed
        // [JsonPropertyName("paypal")]
        // public PaypalDetails? Paypal { get; set; }
    }

    public class CardDetails
    {
        [JsonPropertyName("card_number")] // Updated from "number"
        public string? CardNumber { get; set; }

        [JsonPropertyName("card_exp_month")] // Updated from "expiry_month"
        public string? CardExpiryMonth { get; set; }

        [JsonPropertyName("card_exp_year")] // Updated from "expiry_year"
        public string? CardExpiryYear { get; set; }

        [JsonPropertyName("card_cvc")] // Updated from "cvc"
        public string? CardCvc { get; set; }

        // Other card details like card_holder_name if needed
    }
    
    public class Address
    {
        // This 'Address' class is used for top-level shipping/billing
        // and also for payment_method_data.billing.
        // The cURL example for payment_method_data.billing.address has line1, city etc. directly under 'address'.
        [JsonPropertyName("address")]
        public AddressDetails? AddressDetails { get; set; } 

        [JsonPropertyName("phone")] // Top-level phone for shipping/billing address block
        public PhoneDetails? Phone { get; set; } 
        
        // For payment_method_data.billing, the cURL does not show a nested phone object.
        // If a phone is needed for payment_method_data.billing, it might be directly under Address or a new model.
    }

    public class AddressDetails 
    {
        [JsonPropertyName("line1")]
        public string? Line1 { get; set; }

        [JsonPropertyName("line2")]
        public string? Line2 { get; set; }

        [JsonPropertyName("line3")]
        public string? Line3 { get; set; }
        
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("zip")]
        public string? Zip { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; } // ISO 2-letter country code

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }
        
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
    }

    public class PhoneDetails // Used for top-level shipping/billing address block's phone
    {
        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }
    }

    public class BrowserInfo
    {
        [JsonPropertyName("color_depth")]
        public int? ColorDepth { get; set; }

        [JsonPropertyName("java_enabled")]
        public bool? JavaEnabled { get; set; }

        [JsonPropertyName("java_script_enabled")]
        public bool? JavaScriptEnabled { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("screen_height")]
        public int? ScreenHeight { get; set; }

        [JsonPropertyName("screen_width")]
        public int? ScreenWidth { get; set; }

        [JsonPropertyName("time_zone")]
        public int? TimeZone { get; set; }

        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; }

        [JsonPropertyName("accept_header")]
        public string? AcceptHeader { get; set; }

        [JsonPropertyName("user_agent")]
        public string? UserAgent { get; set; }
    }

    public class OrderDetailItem
    {
        [JsonPropertyName("product_name")]
        public string? ProductName { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("amount")]
        public long? Amount { get; set; } // Assuming this is also in smallest currency unit

        [JsonPropertyName("product_img_link")]
        public string? ProductImgLink { get; set; }
    }
}
