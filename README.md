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
*   **Hyperswitch Account & API Key:** You will need an active Hyperswitch account and your secret API key to make calls to the API. You can obtain these from your Hyperswitch dashboard.

## Setup & Installation

To use this SDK in your .NET application, you will need to reference it directly from its source code.

1.  **Obtain the SDK Source Code:**
    *   Ensure you have the complete source code for the `Hyperswitch.Sdk` project (e.g., by cloning its repository or having it as part of your solution). The `Hyperswitch.Sdk` folder should be accessible from your consuming project.

2.  **Add a Project Reference:**
    *   In your consuming .NET project (e.g., a .NET Console App, Web API, etc.), you need to add a reference to the `Hyperswitch.Sdk.csproj` file.
    *   **Using Visual Studio:**
        1.  In Solution Explorer, right-click on your project's "Dependencies".
        2.  Select "Add Project Reference..."
        3.  In the Reference Manager dialog, click "Browse..." and navigate to the location of the `Hyperswitch.Sdk` folder, then select the `Hyperswitch.Sdk.csproj` file. Click "OK".
    *   **Using .NET CLI:**
        1.  Open your terminal or command prompt.
        2.  Navigate to the directory of your consuming project (the one that will use the SDK).
        3.  Run the following command, adjusting the path to `Hyperswitch.Sdk.csproj` as necessary relative to your project's location:
            ```bash
            dotnet add reference path/to/Hyperswitch.Sdk/Hyperswitch.Sdk.csproj
            ```
            *(Example: `dotnet add reference ../Hyperswitch.Sdk/Hyperswitch.Sdk.csproj` if your consuming project and the `Hyperswitch.Sdk` folder are sibling directories.)*

3.  **Target Framework:**
    *   This SDK targets `.NET 9.0`. Ensure your consuming project is compatible.

After adding the project reference, you can use the SDK's namespaces, classes, and methods in your project by adding `using` statements:
```csharp
using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Services;
using Hyperswitch.Sdk.Models;
using Hyperswitch.Sdk.Exceptions; // For HyperswitchApiException
```
The SDK source code includes XML documentation comments (`/// <summary>...`) which can provide IntelliSense in your IDE if your project and the SDK project are part of the same solution.

## Getting Started

### Initialization

To begin using the SDK, you need to initialize the `HyperswitchClient` with your API key and the base URL for the Hyperswitch API environment you are targeting. Then, you can instantiate the service classes you need.

```csharp
using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Services;
// ... other necessary usings ...

public class MyHyperswitchIntegration
{
    private readonly HyperswitchClient _apiClient;
    public readonly PaymentService Payments;
    public readonly RefundService Refunds;
    public readonly CustomerService Customers;
    public readonly MerchantService Merchant;

    public MyHyperswitchIntegration(string apiKey, string hyperswitchApiBaseUrl = "https://sandbox.hyperswitch.io")
    {
        // It's recommended to fetch the API key from a secure configuration source, not hardcode it.
        _apiClient = new HyperswitchClient(hyperswitchApiBaseUrl, apiKey);
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
// var hyperswitchService = new MyHyperswitchIntegration("YOUR_API_KEY_HERE"); 
// await hyperswitchService.CreateSomePayment();
```
**Note:** Always use the Sandbox URL (`https://sandbox.hyperswitch.io`) for testing and development. Replace it with the live URL for production environments.

## Core Concepts

### API Client
*   **`HyperswitchClient`**: This is the central class responsible for making HTTP requests to the Hyperswitch API. It handles authentication (using your API key), request serialization, and response deserialization.

### Service Classes
The SDK is organized into service classes, each corresponding to a major resource:
*   `PaymentService`
*   `RefundService`
*   `CustomerService`
*   `MerchantService`

### Request & Response Models
All API operations use strongly-typed C# classes for request parameters and response data, located in `Hyperswitch.Sdk.Models`.

### Asynchronous Operations
All API methods are asynchronous (`async Task` or `async Task<T>`). Use `await` for non-blocking calls.

## API Reference & Usage Examples

Below is a summary of available services and key methods. Refer to the source code (with XML comments) or official Hyperswitch API documentation for full details on request/response models.

---

### `PaymentService`
Manages payment intents.

*   **`CreateAsync(PaymentIntentRequest request)`**: Creates a payment.
    *   Example:
        ```csharp
        var paymentRequest = new PaymentIntentRequest
        {
            Amount = 2000, // 20.00 (smallest currency unit)
            Currency = "USD",
            Confirm = true, // Attempt to confirm immediately
            // ProfileId = "YOUR_PROFILE_ID_HERE", // Optional, if needed
            // ... other parameters like PaymentMethodData, CustomerId, ReturnUrl
        };
        PaymentIntentResponse? payment = await Payments.CreateAsync(paymentRequest);
        ```
*   **`RetrieveAsync(string paymentId)`**: Gets payment details.
*   **`SyncPaymentStatusAsync(string paymentId, string? clientSecret = null, bool forceSync = false)`**: Gets latest payment status.
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

---

### `CustomerService`
Manages customers and their payment methods.

*   **`CreateCustomerAsync(CustomerRequest request)`**: Creates a customer.
*   **`RetrieveCustomerAsync(string customerId)`**: Gets customer details.
*   **`UpdateCustomerAsync(string customerId, CustomerUpdateRequest request)`**: Updates a customer.
*   **`DeleteCustomerAsync(string customerId)`**: Deletes a customer.
*   **`ListCustomersAsync(CustomerListRequest? request = null)`**: Lists customers.
*   **`ListPaymentMethodsAsync(string customerId)`**: Lists a customer's saved payment methods.

---

### `MerchantService`
Merchant-level operations.

*   **`ListAvailablePaymentMethodsAsync(MerchantPMLRequest? request = null)`**: Lists merchant's available payment methods.
    *   Uses `GET /account/payment_methods`.
    *   **Note:** Based on testing, this endpoint might require a specific `profile_id` to be implicitly available via your API key or explicitly sent if your account has multiple profiles. If you encounter a "Profile id not found" error, ensure your API key context is correct or try providing a `ProfileId` in the `MerchantPMLRequest`.
    *   Example:
        ```csharp
        var pmlRequest = new MerchantPMLRequest 
        { 
            // ProfileId = "YOUR_PROFILE_ID_HERE", // May be required depending on API key setup
            // Country = "US", 
            // Currency = "USD" 
        };
        MerchantPMLResponse? pml = await Merchant.ListAvailablePaymentMethodsAsync(pmlRequest);
        if (pml?.PaymentMethods != null) { /* Process available payment methods */ }
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
}
```

## Sample Application

The SDK includes a sample console application (`Hyperswitch.Sdk.Sample`) demonstrating various SDK features.

**To run the sample:**

1.  Navigate to `Hyperswitch.Sdk.Sample` directory.
2.  Open `Program.cs` and replace `YOUR_API_KEY_HERE` (placeholder for `apiKey` variable) with your actual Hyperswitch secret API key.
3.  Run: `dotnet run`

## Contributing
For contributions, please contact the Hyperswitch team or refer to any contribution guidelines provided with the source repository.

## License
This SDK is licensed under the MIT License.
