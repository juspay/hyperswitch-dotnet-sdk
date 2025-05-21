# Hyperswitch .NET SDK

## Overview

The Hyperswitch .NET SDK provides a convenient way for .NET developers to integrate server-side with Hyperswitch APIs. This library simplifies interactions for managing payments, refunds, and listing customer payment methods within your .NET applications.

## Features

*   **Typed Client:** A `HyperswitchClient` handles API communication, including authentication and request/response processing.
*   **Service-Oriented Design:** Dedicated service classes (`PaymentService`, `RefundService`, `CustomerService`) for logical grouping of API operations.
*   **Comprehensive API Coverage:**
    *   **Payments:** Create, Retrieve/Sync Status, Confirm, Capture, Update, and Cancel.
    *   **Refunds:** Create, Retrieve, Update, and List.
    *   **Customer:** List saved payment methods.
*   **Strongly-Typed Models:** C# classes for all API request and response objects, ensuring type safety and ease of use (located in `Hyperswitch.Sdk.Models`).
*   **Asynchronous Operations:** All API calls are `async Task` based for non-blocking I/O.
*   **Custom Exception Handling:** `HyperswitchApiException` provides detailed error information from the API.

## Setup & Installation

This SDK is currently set up to be consumed as a direct project reference. If it were published as a NuGet package in the future, installation would typically be via `dotnet add package Hyperswitch.Sdk`.

For now, to use this SDK in your .NET application:

1.  **Obtain the SDK Source Code:**
    *   Ensure you have the complete source code for the `Hyperswitch.Sdk` project. This usually means cloning the repository that contains this SDK. The `Hyperswitch.Sdk` folder should be accessible from your consuming project.

2.  **Add a Project Reference:**
    *   In your consuming .NET project (e.g., a .NET Console App, Web API, etc.), you need to add a reference to the `Hyperswitch.Sdk.csproj` file.
    *   **Using Visual Studio:**
        1.  In Solution Explorer, right-click on your project's "Dependencies" (or "References" in older project types).
        2.  Select "Add Project Reference..."
        3.  In the Reference Manager dialog, click "Browse..." and navigate to the location of the `Hyperswitch.Sdk` folder, then select the `Hyperswitch.Sdk.csproj` file. Click "OK".
    *   **Using .NET CLI:**
        1.  Open your terminal or command prompt.
        2.  Navigate to the directory of your consuming project (the one that will use the SDK).
        3.  Run the following command, adjusting the path to `Hyperswitch.Sdk.csproj` as necessary relative to your project's location:
            ```bash
            dotnet add reference ../Hyperswitch.Sdk/Hyperswitch.Sdk.csproj
            ```
            *(This example assumes your consuming project and the `Hyperswitch.Sdk` folder are sibling directories within a common parent folder.)*

3.  **Target Framework:**
    *   This SDK targets `.NET 9.0`. Ensure your consuming project is compatible (e.g., also targets .NET 9.0 or a compatible newer version). If needed, you might have to adjust the target framework in `Hyperswitch.Sdk.csproj` and recompile the SDK.

Once the reference is added, you can use the SDK's namespaces, classes, and methods in your project by adding `using` statements, for example:
```csharp
using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Services;
using Hyperswitch.Sdk.Models;
```

## Core Concepts

*   **`HyperswitchClient`:** This is the central component. You instantiate it with your Hyperswitch API base URL (e.g., `https://sandbox.hyperswitch.io`) and your secret API key. It manages the underlying `HttpClient` and authentication headers.
*   **Service Classes:** For each main API resource (Payments, Refunds, Customers), there's a corresponding service class:
    *   `PaymentService`
    *   `RefundService`
    *   `CustomerService`
    These services take an instance of `HyperswitchClient` in their constructor and provide methods for specific API endpoints (e.g., `paymentService.CreateAsync(...)`).
*   **Models:** All data sent to or received from the API is structured using C# classes found in the `Hyperswitch.Sdk.Models` namespace. These provide IntelliSense and type checking.

## Usage Examples

### 1. Initialization

First, include the necessary `using` statements and initialize the `HyperswitchClient` and the service(s) you intend to use:

```csharp
using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Services;
using Hyperswitch.Sdk.Models;      // For request/response objects
using Hyperswitch.Sdk.Exceptions;  // For HyperswitchApiException
using System;                       // For Console.WriteLine
using System.Threading.Tasks;       // For async Task
using System.Collections.Generic; // For Dictionary, List

// ... within your application class or method ...

public class MyHyperswitchIntegrator
{
    private readonly PaymentService _paymentService;
    private readonly RefundService _refundService;
    private readonly CustomerService _customerService;

    public MyHyperswitchIntegrator(string apiKey, string baseUrl = "https://sandbox.hyperswitch.io")
    {
        var apiClient = new HyperswitchClient(baseUrl, apiKey);
        _paymentService = new PaymentService(apiClient);
        _refundService = new RefundService(apiClient);
        _customerService = new CustomerService(apiClient);
    }

    // Example method to use a service
    public async Task ExampleCreatePaymentAsync()
    {
        // See API call examples below
    }
}
```

### 2. Making API Calls

All service methods are asynchronous.

#### Example: Create a Payment

```csharp
public async Task<PaymentIntentResponse?> CreatePaymentExampleAsync()
{
    var paymentRequest = new PaymentIntentRequest
    {
        Amount = 2000, // e.g., 20.00 USD (amount in cents)
        Currency = "USD",
        Email = "customer@example.com",
        Description = "Order #12345",
        Confirm = true,             // Create and confirm in one go
        CaptureMethod = "automatic",  // Auto-capture after successful authentication
        PaymentMethod = "card",
        PaymentMethodType = "credit",
        PaymentMethodData = new PaymentMethodData
        {
            Card = new CardDetails
            {
                CardNumber = "4917610000000000", // Use a test card number
                CardExpiryMonth = "12",
                CardExpiryYear = "2030",
                CardCvc = "123"
            }
        },
        ReturnUrl = "https://example.com/return_url_for_3ds_redirect"
        // Add other fields like Billing, Shipping, BrowserInfo as needed
    };

    try
    {
        PaymentIntentResponse? paymentResponse = await _paymentService.CreateAsync(paymentRequest);

        if (paymentResponse != null)
        {
            Console.WriteLine($"Payment Status: {paymentResponse.Status}");
            Console.WriteLine($"Payment ID: {paymentResponse.PaymentId}");

            if (paymentResponse.Status == "requires_customer_action" && !string.IsNullOrEmpty(paymentResponse.NextAction?.RedirectToUrl))
            {
                Console.WriteLine($"Redirect customer to: {paymentResponse.NextAction.RedirectToUrl}");
                // In a web application, you would redirect the user's browser to this URL.
            }
            else if (paymentResponse.Status == "succeeded")
            {
                Console.WriteLine("Payment successful!");
            }
            // Handle other statuses as needed
            return paymentResponse;
        }
        else
        {
            Console.WriteLine("Failed to create payment or response was null.");
            return null;
        }
    }
    catch (HyperswitchApiException apiEx)
    {
        Console.WriteLine($"API Error creating payment: {apiEx.Message}");
        Console.WriteLine($"  Status Code: {apiEx.StatusCode}");
        if (apiEx.ErrorDetails?.Error != null)
        {
            Console.WriteLine($"  API Code: {apiEx.ErrorDetails.Error.Code}");
            Console.WriteLine($"  API Message: {apiEx.ErrorDetails.Error.Message}");
        }
        Console.WriteLine($"  Raw Response: {apiEx.ResponseContent}");
        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        return null;
    }
}
```

### 3. Error Handling

API errors will throw a `HyperswitchApiException`. Catch this exception to handle errors gracefully:

```csharp
try
{
    // Make an API call, e.g., await _paymentService.RetrieveAsync("invalid_payment_id");
}
catch (HyperswitchApiException apiEx)
{
    Console.WriteLine($"An API error occurred:");
    Console.WriteLine($"  HTTP Status Code: {apiEx.StatusCode}");
    Console.WriteLine($"  Raw Response Body: {apiEx.ResponseContent}"); // Contains the JSON error from Hyperswitch

    if (apiEx.ErrorDetails?.Error != null)
    {
        Console.WriteLine($"  Hyperswitch Error Code: {apiEx.ErrorDetails.Error.Code}");
        Console.WriteLine($"  Hyperswitch Error Message: {apiEx.ErrorDetails.Error.Message}");
    }
}
catch (System.Net.Http.HttpRequestException httpEx)
{
    // Handle network errors or issues before a response is received
    Console.WriteLine($"Network error: {httpEx.Message}");
}
catch (Exception ex)
{
    // Handle other unexpected errors
    Console.WriteLine($"An unexpected error: {ex.Message}");
}
```

## Available Services and Key Operations

### `PaymentService`
*   `CreateAsync(PaymentIntentRequest request)`: Creates a new payment.
*   `RetrieveAsync(string paymentId)`: Retrieves details of a specific payment.
*   `SyncPaymentStatusAsync(string paymentId, string? clientSecret = null, bool forceSync = false)`: Retrieves the latest status, optionally forcing a sync.
*   `ConfirmPaymentAsync(string paymentId, PaymentConfirmRequest? confirmRequest = null)`: Confirms a payment.
*   `CapturePaymentAsync(string paymentId, PaymentCaptureRequest? captureRequest = null)`: Captures an authorized payment.
*   `UpdatePaymentAsync(string paymentId, PaymentUpdateRequest updateRequest)`: Updates details of a payment.
*   `CancelPaymentAsync(string paymentId, PaymentCancelRequest? cancelRequest = null)`: Cancels (voids) an authorized payment.

### `RefundService`
*   `CreateRefundAsync(RefundCreateRequest request)`: Creates a refund for a payment.
*   `RetrieveRefundAsync(string refundId)`: Retrieves details of a specific refund.
*   `UpdateRefundAsync(string refundId, RefundUpdateRequest request)`: Updates details of a refund.
*   `ListRefundsAsync(RefundListRequest? request = null)`: Lists refunds, with optional filters (uses `POST /refunds/list`).

### `CustomerService`
*   `ListPaymentMethodsAsync(string customerId)`: Lists saved payment methods for a customer (uses `GET /customers/{customerId}/payment_methods`).

Refer to the method signatures within each service class and the corresponding model classes in `Hyperswitch.Sdk.Models` for detailed request and response structures.
