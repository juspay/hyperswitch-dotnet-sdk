# Hyperswitch .NET SDK

The official Hyperswitch .NET SDK provides .NET developers with a simple and convenient way to integrate their server-side applications with Hyperswitch APIs. This library allows you to easily manage payments, refunds, customers, and more, directly from your .NET code.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Setup & Installation](#setup--installation)
- [Getting Started](#getting-started)
  - [Initialization](#initialization)
- [Core Concepts](#core-concepts)
  - [API Client](#api-client)
  - [Service Classes](#service-classes)
  - [Request & Response Models](#request--response-models)
  - [Asynchronous Operations](#asynchronous-operations)
- [API Reference & Usage Examples](#api-reference--usage-examples)
  - [PaymentService](#paymentservice)
  - [RefundService](#refundservice)
  - [CustomerService](#customerservice)
  - [MerchantService](#merchantservice)
- [Error Handling](#error-handling)
- [Sample Application](#sample-application)
- [Contributing](#contributing)
- [License](#license)

## Prerequisites

*   **.NET 9.0 SDK or later:** This SDK targets `net9.0`. Ensure your development environment has the .NET 9.0 SDK installed. Your project should also target a compatible framework version (e.g., `net9.0`).
*   **Hyperswitch Account & API Keys:** You will need an active Hyperswitch account.
    *   **Secret API Key:** Required for most server-side operations.
    *   **Publishable API Key:** Required for specific client-side oriented operations if you intend to use them via the SDK (e.g., listing payment methods using a `client_secret`).
    *   **Profile ID:** You might need one or more Profile IDs configured in your Hyperswitch account. The SDK allows setting a default Profile ID.
    *   Obtain these from your Hyperswitch dashboard.

## Setup & Installation

Install the SDK using the .NET CLI:
```bash
dotnet add package Juspay.Hyperswitch.Sdk --version 1.0.1
```

Or via the NuGet Package Manager console:
```powershell
Install-Package Juspay.Hyperswitch.Sdk -Version 1.0.1
```

After installing the package, you can use the SDK's namespaces:
```csharp
using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Services;
using Hyperswitch.Sdk.Models;
using Hyperswitch.Sdk.Exceptions; // For HyperswitchApiException
```
The SDK source code includes XML documentation comments for IntelliSense.

## Getting Started

### Initialization

To begin using the SDK, initialize the `HyperswitchClient`. You must provide your **Secret API Key**. Optionally, you can also provide a **Publishable API Key** (if you intend to use client-side flows like PML with a `client_secret`) and a **Default Profile ID** (which will be used if a `ProfileId` is not specified in individual requests that support it).

```csharp
using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Services;
// ... other necessary usings ...

public class MyHyperswitchService
{
    private readonly HyperswitchClient _apiClient;
    public readonly PaymentService Payments;
    public readonly RefundService Refunds;
    public readonly CustomerService Customers;
    public readonly MerchantService Merchant;

    public MyHyperswitchService(string secretKey, string? publishableKey, string? defaultProfileId, string hyperswitchApiBaseUrl = "https://sandbox.hyperswitch.io")
    {
        // It's recommended to fetch API keys and Profile ID from a secure configuration source.
        _apiClient = new HyperswitchClient(
            secretKey: secretKey, 
            publishableKey: publishableKey, 
            defaultProfileId: defaultProfileId, 
            baseUrl: hyperswitchApiBaseUrl
        );
        
        Payments = new PaymentService(_apiClient);
        Refunds = new RefundService(_apiClient);
        Customers = new CustomerService(_apiClient);
        Merchant = new MerchantService(_apiClient);
    }

    // Example method
    public async Task CreateSomePayment()
    {
        // See API call examples below
    }
}

// How to use it in your application:
// var hyperswitchService = new MyHyperswitchService(
//     "YOUR_SECRET_API_KEY", 
//     "YOUR_PUBLISHABLE_API_KEY_IF_NEEDED", 
//     "YOUR_DEFAULT_PROFILE_ID_IF_ANY"
// ); 
// await hyperswitchService.CreateSomePayment();
```
**Note:** Always use the Sandbox URL (`https://sandbox.hyperswitch.io`) for testing and development. Replace it with the live URL for production environments. 
When running examples that involve card payments (e.g., providing `PaymentMethodData` with `CardDetails`), ensure you use valid test card numbers to simulate different scenarios successfully.

## Core Concepts

### API Client
*   **`HyperswitchClient`**: The central class for HTTP requests to the Hyperswitch API. It handles:
    *   Authentication using your API keys.
    *   Management of a default Profile ID.
    *   Request serialization and response deserialization.
    *   Selection between secret and publishable keys for certain operations.

### Service Classes
The SDK is organized into service classes, each corresponding to a major resource:
*   `PaymentService`
*   `RefundService`
*   `CustomerService`
*   `MerchantService`

Each service is initialized with the `HyperswitchClient` instance.

### Request & Response Models
All API operations use strongly-typed C# classes for request parameters and response data, located in the `Hyperswitch.Sdk.Models` namespace.

### Asynchronous Operations
All API methods are asynchronous (`async Task` or `async Task<T>`). Use `await` for non-blocking calls.

## API Reference & Usage Examples

Below is a summary of available services and key methods. Refer to the source code (with XML comments) or official Hyperswitch API documentation for full details on request/response models.

---

### `PaymentService`
Manages payment intents.

*   **`CreateAsync(PaymentIntentRequest request)`**: Creates a payment.
    *   If `request.ProfileId` is not set, the `DefaultProfileId` from `HyperswitchClient` (if configured) will be used.
    *   Example:
        ```csharp
        var paymentRequest = new PaymentIntentRequest
        {
            Amount = 2000, // 20.00 (smallest currency unit)
            Currency = "USD",
            Confirm = true, // Attempt to confirm immediately
            // ProfileId = "prof_specific", // Overrides default if set
            Description = "Standard one-time payment",
            Email = "customer@example.com",
            PaymentMethod = "card", // Example
            PaymentMethodData = new PaymentMethodData { /* ... */ }
            // ... other parameters like ReturnUrl
        };
        PaymentIntentResponse? payment = await Payments.CreateAsync(paymentRequest);
        ```
    *   **Example for Mandate Setup (Initial CIT):**
        ```csharp
        var mandateSetupRequest = new PaymentIntentRequest
        {
            Amount = 1000,
            Currency = "USD",
            Confirm = true,
            CustomerId = "cus_xxxxxxxxxxxx", // Required for saving payment method
            SetupFutureUsage = "off_session", // Indicates intent to save for future off-session use
            Description = "Initial payment for setting up a mandate",
            PaymentMethod = "card",
            PaymentMethodType = "credit",
            PaymentMethodData = new PaymentMethodData { /* ... card details ... */ },
            AuthenticationType = "no_three_ds", // Or other appropriate types
            ReturnUrl = "https://example.com/mandate_setup_return",
            CustomerAcceptance = new CustomerAcceptance 
                { 
                    AcceptanceType = "offline", // or "online"
                    // Online = new OnlineMandate { IpAddress = "x.x.x.x", UserAgent = "..." } // If online
                },
            BrowserInfo = new BrowserInfo { /* ... browser details ... */ }
        };
        PaymentIntentResponse? mandateSetupResponse = await Payments.CreateAsync(mandateSetupRequest);
        // Store mandateSetupResponse.PaymentMethodId for subsequent payments.
        ```
    *   **Example for Subsequent Mandate Payment (MIT):**
        ```csharp
        var recurringPaymentRequest = new PaymentIntentRequest
        {
            Amount = 1500, // Amount for this specific recurring charge
            Currency = "USD",
            Confirm = true,
            CustomerId = "cus_xxxxxxxxxxxx", // Same customer
            OffSession = true, // Indicates this is an MIT
            RecurringDetails = new RecurringDetailsInfo
            {
                Type = "payment_method_id",
                Data = "pm_xxxxxxxxxxxxxx" // The PaymentMethodId from mandateSetupResponse
            },
            Description = "Recurring subscription charge"
        };
        PaymentIntentResponse? recurringPayment = await Payments.CreateAsync(recurringPaymentRequest);
        ```
*   **`RetrieveAsync(string paymentId)`**: Gets payment details.
*   **`SyncPaymentStatusAsync(string paymentId, bool forceSync = false)`**: Gets latest payment status.
*   **`ConfirmPaymentAsync(string paymentId, PaymentConfirmRequest? confirmRequest = null)`**: Confirms a payment.
*   **`CapturePaymentAsync(string paymentId, PaymentCaptureRequest? captureRequest = null)`**: Captures an authorized payment.
*   **`UpdatePaymentAsync(string paymentId, PaymentUpdateRequest updateRequest)`**: Updates a payment.
*   **`CancelPaymentAsync(string paymentId, PaymentCancelRequest? cancelRequest = null)`**: Voids/cancels a payment.

---

### `RefundService`
Manages refunds.

*   **`CreateRefundAsync(RefundCreateRequest request)`**: Creates a refund.
    *   Example:
        ```csharp
        var refundRequest = new RefundCreateRequest
        {
            PaymentId = "pay_xxxxxxxxxxxx",
            Amount = 500, // Refund 5.00 (smallest currency unit)
            Reason = "OTHER" // Valid reasons: OTHER, RETURN, DUPLICATE, FRAUD, CUSTOMER_REQUEST
        };
        RefundResponse? refund = await Refunds.CreateRefundAsync(refundRequest);
        ```
*   **`RetrieveRefundAsync(string refundId)`**: Gets refund details.
*   **`UpdateRefundAsync(string refundId, RefundUpdateRequest request)`**: Updates a refund.
*   **`ListRefundsAsync(RefundListRequest? request = null)`**: Lists refunds.
    *   If `request.ProfileId` is not set, the `DefaultProfileId` from `HyperswitchClient` (if configured) will be used.

---

### `CustomerService`
Manages customers and their payment methods.

*   **`CreateCustomerAsync(CustomerRequest request)`**: Creates a customer.
*   **`RetrieveCustomerAsync(string customerId)`**: Gets customer details.
*   **`UpdateCustomerAsync(string customerId, CustomerUpdateRequest request)`**: Updates a customer.
*   **`DeleteCustomerAsync(string customerId)`**: Deletes a customer.
*   **`ListCustomersAsync(CustomerListRequest? request = null)`**: Lists customers.
*   **`ListPaymentMethodsAsync(string customerId)`**: Lists a customer's saved payment methods.
*   **`ListPaymentMethodsForClientSecretAsync(string clientSecret)`**: Lists a customer's saved payment methods using a client secret.

---

### `MerchantService`
Merchant-level operations.

*   **`ListAvailablePaymentMethodsAsync(MerchantPMLRequest? request = null)`**: Lists merchant's available payment methods (`GET /account/payment_methods`). This method behaves differently based on the request and client configuration:
    *   **Server-Side Usage (Secret Key):**
        *   If `request.ClientSecret` is **NOT** provided.
        *   The SDK uses the **Secret API Key**.
        *   It filters by `Country`, `Currency`, `Amount`, and `ProfileId`.
        *   If `request.ProfileId` is not set, the `DefaultProfileId` from `HyperswitchClient` (if configured) will be used.
        *   Example:
            ```csharp
            var pmlRequest = new MerchantPMLRequest 
            { 
                // ProfileId = "prof_specific_for_secret_key_call", // Optional, uses default if not set
                Country = "US", 
                Currency = "USD" 
            };
            MerchantPMLResponse? pml = await Merchant.ListAvailablePaymentMethodsAsync(pmlRequest);
            ```
    *   **Client-Side Flow Emulation (Publishable Key):**
        *   If `request.ClientSecret` **IS** provided.
        *   The SDK uses the **Publishable API Key** (must be configured in `HyperswitchClient`).
        *   The query will *only* include `client_secret`. Other parameters in `MerchantPMLRequest` (like Country, Amount, ProfileId) are ignored for this specific call type.
        *   This is useful if the server needs to fetch payment methods on behalf of a client that has obtained a `client_secret` (e.g., from a Payment Intent).
        *   Example:
            ```csharp
            // Assuming paymentIntent.ClientSecret is available
            var pmlRequestClientSecret = new MerchantPMLRequest 
            { 
                ClientSecret = paymentIntent.ClientSecret 
            };
            MerchantPMLResponse? pml = await Merchant.ListAvailablePaymentMethodsAsync(pmlRequestClientSecret);
            ```

---

## Error Handling

API errors throw a `HyperswitchApiException`.
*   `StatusCode`: HTTP status code.
*   `ResponseContent`: Raw JSON error response.
*   `ErrorDetails`: Deserialized `ErrorResponse` object (`Error.Code`, `Error.Message`).

**Example:**
```csharp
try
{
    // API call
}
catch (HyperswitchApiException apiEx)
{
    Console.WriteLine($"API Error: {apiEx.Message} (Status: {apiEx.StatusCode})");
    if (apiEx.ErrorDetails?.Error != null)
    {
        Console.WriteLine($"  Code: {apiEx.ErrorDetails.Error.Code}, Details: {apiEx.ErrorDetails.Error.Message}");
    }
    // Log apiEx.ResponseContent for full error details from the API
}
```

## Sample Application

The SDK includes a sample console application (`Hyperswitch.Sdk.Sample`) demonstrating various SDK features.

**To run the sample:**

1.  Navigate to the `Hyperswitch.Sdk.Sample` directory.
2.  Open `Program.cs`. At the beginning of the `Main` method, configure the following string variables:
    *   `secretKey`: Your Hyperswitch Secret API Key.
    *   `publishableKey`: Your Hyperswitch Publishable API Key (can be `null` if not testing PML with client_secret).
    *   `defaultProfileId`: Your default Hyperswitch Profile ID (can be `null` if you always specify it in requests).
3.  Run the sample application: `dotnet run`

## Contributing
For contributions, please contact the Hyperswitch team or refer to any contribution guidelines provided with the source repository.

## License
This SDK is licensed under the MIT License.
