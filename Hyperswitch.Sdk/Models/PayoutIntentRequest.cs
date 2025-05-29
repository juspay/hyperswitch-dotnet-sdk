using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models;

/// <summary>
/// Represents a payout request to the Hyperswitch API.
/// </summary>
public class PayoutIntentRequest
{
    /// <summary>
    /// The amount in the smallest currency unit (e.g., cents).
    /// </summary>
    [Required]
    public required int Amount { get; set; }

    /// <summary>
    /// ISO currency code (e.g., "USD").
    /// </summary>
    [Required]
    public required string Currency { get; set; }

    /// <summary>
    /// Payout routing information.
    /// </summary>
    public Routing? Routing { get; set; }

    /// <summary>
    /// List of connector names to prioritize.
    /// </summary>
    public List<string>? Connector { get; set; }

    /// <summary>
    /// Confirm the payout immediately.
    /// </summary>
    public bool Confirm { get; set; } = false;

    /// <summary>
    /// Type of payout.
    /// </summary>
    [Required]
    public required PayoutType PayoutType { get; set; }

    /// <summary>
    /// Data related to payout method.
    /// </summary>
    [Required]
    public required PayoutMethodData PayoutMethodData { get; set; }

    /// <summary>
    /// Billing address and contact.
    /// </summary>
    public Billing? Billing { get; set; }

    /// <summary>
    /// Auto-fulfill on creation.
    /// </summary>
    public bool AutoFulfill { get; set; } = false;

    /// <summary>
    /// ID of the customer.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Full customer object.
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// Redirection URL after payout completion.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Country of the business (ISO Alpha-2).
    /// </summary>
    public string? BusinessCountry { get; set; }

    /// <summary>
    /// Business label.
    /// </summary>
    public string? BusinessLabel { get; set; }

    /// <summary>
    /// Free text description of the payout.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Legal entity type.
    /// </summary>
    public EntityType? EntityType { get; set; }

    /// <summary>
    /// Is this a recurring payout?
    /// </summary>
    public bool Recurring { get; set; } = false;

    /// <summary>
    /// Metadata key-value pairs.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Token to reference the payout.
    /// </summary>
    public string? PayoutToken { get; set; }

    /// <summary>
    /// Profile ID used for merchant config scoping.
    /// </summary>
    public string? ProfileId { get; set; }

    /// <summary>
    /// Payout processing priority.
    /// </summary>
    public Priority? Priority { get; set; }

    /// <summary>
    /// Generate payout link.
    /// </summary>
    public bool PayoutLink { get; set; } = false;

    /// <summary>
    /// Configuration for payout link.
    /// </summary>
    public PayoutLinkConfig? PayoutLinkConfig { get; set; }

    /// <summary>
    /// Expiry time in seconds for session.
    /// </summary>
    public int? SessionExpiry { get; set; }

    /// <summary>
    /// Email of payout recipient.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Name of payout recipient.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Phone number of payout recipient.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Phone country code (e.g., +1).
    /// </summary>
    public string? PhoneCountryCode { get; set; }

    /// <summary>
    /// Existing payout method reference.
    /// </summary>
    public string? PayoutMethodId { get; set; }
}

/// <summary>
/// Represents payout routing settings.
/// </summary>
public class Routing
{
    /// <summary>
    /// Routing method type.
    /// </summary>
    [Required]
    public required RoutingType Type { get; set; }

    /// <summary>
    /// Routing configuration details.
    /// </summary>
    [Required]
    public required RoutingData Data { get; set; }
}

/// <summary>
/// Represents the routing data configuration for a payout.
/// </summary>
public class RoutingData
{
    /// <summary>
    /// Connector identifier.
    /// </summary>
    [Required]
    public required string Connector { get; set; }

    /// <summary>
    /// Merchant connector account ID.
    /// </summary>
    [Required]
    public required string MerchantConnectorId { get; set; }
}


/// <summary>
/// Represents payout method-specific data.
/// </summary>
public class PayoutMethodData
{
    /// <summary>
    /// Card payout data.
    /// </summary>
    public Card? Card { get; set; }

    /// <summary>
    /// Bank transfer payout data.
    /// </summary>
    public Bank? Bank { get; set; }
}

/// <summary>
/// Represents bank details used for payout.
/// </summary>
public class Bank
{
    /// <summary>
    /// Bank account number.
    /// </summary>
    public string? AccountNumber { get; set; }

    /// <summary>
    /// Bank routing number.
    /// </summary>
    public string? RoutingNumber { get; set; }

    /// <summary>
    /// Bank name.
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// Bank city.
    /// </summary>
    public string? BankCity { get; set; }

    /// <summary>
    /// Bank country code (ISO Alpha-2).
    /// </summary>
    public string? BankCountryCode { get; set; }

    /// <summary>
    /// IBAN for SEPA payouts.
    /// </summary>
    public string? Iban { get; set; }

    /// <summary>
    /// BIC/SWIFT code.
    /// </summary>
    public string? Bic { get; set; }

    /// <summary>
    /// Sort code for BACS payouts.
    /// </summary>
    public string? SortCode { get; set; }
}

/// <summary>
/// Represents card details used for payout.
/// </summary>
public class Card
{
    /// <summary>
    /// Card number.
    /// </summary>
    [Required]
    public required string CardNumber { get; set; }

    /// <summary>
    /// Expiry month (MM).
    /// </summary>
    [Required]
    public required string ExpiryMonth { get; set; }

    /// <summary>
    /// Expiry year (YYYY).
    /// </summary>
    [Required]
    public required string ExpiryYear { get; set; }

    /// <summary>
    /// Card holder name.
    /// </summary>
    [Required]
    public required string CardHolderName { get; set; }
}

/// <summary>
/// Represents billing information for the payout.
/// </summary>
public class Billing
{
    /// <summary>
    /// Billing address.
    /// </summary>
    public PayoutAddress? Address { get; set; }

    /// <summary>
    /// Billing phone info.
    /// </summary>
    public PayoutPhoneDetails? Phone { get; set; }

    /// <summary>
    /// Billing email.
    /// </summary>
    public string? Email { get; set; }
}

/// <summary>
/// Represents address structure.
/// </summary>
public class PayoutAddress
{
    /// <summary>
    /// City name.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Country code (ISO Alpha-2).
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Address line 1.
    /// </summary>
    public string? Line1 { get; set; }

    /// <summary>
    /// Address line 2.
    /// </summary>
    public string? Line2 { get; set; }

    /// <summary>
    /// Address line 3.
    /// </summary>
    public string? Line3 { get; set; }

    /// <summary>
    /// Postal code.
    /// </summary>
    public string? Zip { get; set; }

    /// <summary>
    /// State or province.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// First name of recipient.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name of recipient.
    /// </summary>
    public string? LastName { get; set; }
}

/// <summary>
/// Represents phone number details.
/// </summary>
public class PayoutPhoneDetails
{
    /// <summary>
    /// Phone number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Phone country code.
    /// </summary>
    public string? CountryCode { get; set; }
}

/// <summary>
/// Represents a customer involved in the payout.
/// </summary>
public class Customer
{
    /// <summary>
    /// Customer ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Customer name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Customer email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Customer phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Country code of customer's phone.
    /// </summary>
    public string? PhoneCountryCode { get; set; }
}

/// <summary>
/// Configuration options for payout link.
/// </summary>
public class PayoutLinkConfig
{
    /// <summary>
    /// Logo image URL.
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Merchant name.
    /// </summary>
    public string? MerchantName { get; set; }

    /// <summary>
    /// UI theme.
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// Payout link ID.
    /// </summary>
    public string? PayoutLinkId { get; set; }

    /// <summary>
    /// Enabled payment methods.
    /// </summary>
    public string? EnabledPaymentMethods { get; set; }

    /// <summary>
    /// Form layout identifier.
    /// </summary>
    public string? FormLayout { get; set; }

    /// <summary>
    /// Whether test mode is enabled.
    /// </summary>
    public bool TestMode { get; set; }
}
/// <summary>
/// Supported payout types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PayoutType
{
    /// <summary>
    /// Card-based payout.
    /// </summary>
    Card,

    /// <summary>
    /// Bank transfer payout.
    /// </summary>
    Bank,

    /// <summary>
    /// Wallet-based payout.
    /// </summary>
    Wallet
}

/// <summary>
/// Routing method type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoutingType
{
    /// <summary>
    /// Use a single routing connector.
    /// </summary>
    Single,

    /// <summary>
    /// Use multiple connectors based on rules.
    /// </summary>
    Multiple
}

/// <summary>
/// Legal entity type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityType
{
    /// <summary>
    /// An individual person.
    /// </summary>
    Individual,

    /// <summary>
    /// A registered company.
    /// </summary>
    Company
}

/// <summary>
/// Payout processing priority.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Priority
{
    /// <summary>
    /// Fastest possible processing.
    /// </summary>
    Instant,

    /// <summary>
    /// Standard processing time.
    /// </summary>
    Standard
}
