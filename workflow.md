# Hyperswitch .NET Server-Side SDK Development Workflow

## 1. Objective
Generate a robust and easy-to-use .NET Server-Side SDK to interact with Hyperswitch Server-to-Server (S2S) APIs.

## 2. Core Requirements & Decisions
*   **Target Platform:** .NET 9.0 (Updated from .NET 8)
*   **Language:** C#
*   **Primary HTTP Client:** `System.Net.Http.HttpClient`
*   **JSON Serialization:** `System.Text.Json`
*   **Authentication:** API Secret Key via `api-key` header (Updated from Authorization: Bearer). Publishable key also supported.
*   **API Focus:** Payments (Create, Retrieve, Confirm, Capture, Update, Cancel, Mandates/Recurring), Refunds, Customers, Merchant Payment Methods, Payouts.
*   **Error Handling:** Custom `HyperswitchApiException` for API errors.
*   **Project Type:** .NET Class Library

## 3. Planned SDK Structure (Files to be generated)
```
Hyperswitch.Sdk/
|-- Hyperswitch.Sdk.csproj
|-- HyperswitchClient.cs       # Core client for API communication, auth, base URL
|-- Models/                    # Request/Response POCOs
|   |-- PaymentIntentRequest.cs
|   |-- PaymentIntentResponse.cs
|   |-- ErrorResponse.cs
|   |-- (others as needed)
|-- Services/                  # Service classes per API resource
|   |-- PaymentService.cs
|   |-- PayoutService.cs
|   |-- (others as needed)
|-- Exceptions/                # Custom exceptions
|   |-- HyperswitchApiException.cs
```

## 4. Development Steps & Log (Code Generation Plan)

**Phase 1: Setup and Core Client Generation**

*   **[X] Step 0: Prerequisite - Ensure .NET SDK is installed and `dotnet` CLI is working.**
    *   *Status:* **COMPLETED** - `dotnet --version` returned `9.0.300`.
*   **[X] Step 1: Initialize .NET Class Library Project & Directory Structure.**
    *   *Action:* Executed `cd Hyperswitch.Sdk && dotnet new classlib -f net9.0 --force && mkdir -p Models Services Exceptions`.
    *   *Status:* **COMPLETED**.
*   **[X] Step 2: Generate `HyperswitchClient.cs`.**
    *   *Content:* Class with constructor for API key/base URL, methods for HTTP GET/POST, authentication header logic.
    *   *Status:* **COMPLETED**.
*   **[X] Step 3: Generate `Exceptions/HyperswitchApiException.cs`.**
    *   *Content:* Custom exception class.
    *   *Status:* **COMPLETED**.
*   **[X] Step 4: Generate `Models/ErrorResponse.cs`.**
    *   *Content:* POCO for API error deserialization.
    *   *Status:* **COMPLETED**.
*   **[X] Step 5: Generate `Models/PaymentIntentRequest.cs`.**
    *   *Content:* Properties based on Hyperswitch "Payments - Create" API.
    *   *Status:* **COMPLETED**.
*   **[X] Step 6: Generate `Models/PaymentIntentResponse.cs`.**
    *   *Content:* Properties based on Hyperswitch "Payments - Create" API response.
    *   *Status:* **COMPLETED**.
*   **[X] Step 7: Generate `Services/PaymentService.cs` (Initial methods).**
    *   *Content:* Class with `CreateAsync` and `RetrieveAsync` methods using `HyperswitchClient`.
    *   *Status:* **COMPLETED**.
*   **[X] Step 8: Add `SyncPaymentStatusAsync` method to `PaymentService.cs` and update sample.**
    *   *Content:* Added `SyncPaymentStatusAsync` with `forceSync` parameter. Updated `Program.cs` to call it.
    *   *Status:* **COMPLETED**.

**Phase 2: Expanding PaymentService and Adding Other Services**
*   **[X] Step 9: Add `ConfirmPaymentAsync`, `CapturePaymentAsync`, `UpdatePaymentAsync`, `CancelPaymentAsync` to `PaymentService.cs`.**
    *   *Models:* `PaymentConfirmRequest.cs`, `PaymentCaptureRequest.cs`, `PaymentUpdateRequest.cs`, `PaymentCancelRequest.cs`.
    *   *Status:* **COMPLETED**.
*   **[X] Step 10: Implement `RefundService` with CRUD operations.**
    *   *Models:* `RefundCreateRequest.cs`, `RefundResponse.cs`, `RefundUpdateRequest.cs`, `RefundListRequest.cs`, `RefundListResponse.cs`.
    *   *Status:* **COMPLETED**.
*   **[X] Step 11: Implement `CustomerService` with CRUD operations.**
    *   *Models:* `CustomerRequest.cs`, `CustomerResponse.cs`, `CustomerUpdateRequest.cs`, `CustomerDeleteResponse.cs`, `CustomerListRequest.cs`, `CustomerListResponse.cs`, `CustomerPaymentMethodListResponse.cs`, `CustomerPaymentMethod.cs`, `CardSummary.cs`.
    *   *Status:* **COMPLETED**.
*   **[X] Step 12: Implement `MerchantService` for listing payment methods.**
    *   *Models:* `MerchantPMLRequest.cs`, `MerchantPMLResponse.cs` (and its dependencies like `PaymentMethodGroup.cs`, `PaymentMethodTypeDetails.cs` - though these were later refactored).
    *   *Status:* **COMPLETED**.

**Phase 3: Model Refinement and Mandate/Recurring Payments**
*   **[X] Step 13: Refactor shared/nested models into individual files.**
    *   *Details:* Moved classes like `Address`, `CardDetails`, `BrowserInfo`, etc., from `PaymentIntentRequest.cs` into their own files under `Models/`.
    *   *Status:* **COMPLETED**.
*   **[X] Step 14: Align SDK models with OpenAPI specification.**
    *   *Details:* Reviewed and updated properties for `PaymentIntentRequest`, `PaymentIntentResponse`, `PaymentConfirmRequest`, `PaymentCaptureRequest`, `RefundCreateRequest`, `RefundResponse`, `CustomerRequest`, `CustomerResponse`, `CustomerDeleteResponse`, etc., to match OpenAPI spec more closely. This included adding missing properties and adjusting types/nullability.
    *   *Status:* **COMPLETED**.
*   **[X] Step 15: Implement Mandate Payment Flow.**
    *   *Models Added/Updated:*
        *   `PaymentIntentRequest.cs`: Added `SetupFutureUsage`, `MandateData`, `OffSession`, `RecurringDetails`.
        *   `MandateData.cs`: Created with `CustomerAcceptance`, `OnlineMandate`, `MandateType`, `MandateAmountData`.
        *   `RecurringDetailsInfo.cs`: Created.
    *   *Sample App:* Added `TestMandatePaymentFlowAsync` to `Program.cs` to demonstrate CIT setup and MIT execution.
    *   *Status:* **COMPLETED**.
*   **[X] Step 16: Refine `MerchantPMLResponse` structure (partially reverted).**
    *   *Details:* Attempted to align `MerchantPMLResponse` with `PaymentMethodListResponse` schema from OpenAPI, creating `ResponsePaymentMethodsEnabled.cs` and `ResponsePaymentMethodTypes.cs`. This was later reverted by user request to keep the existing simpler structure (`PaymentMethodGroup`, `PaymentMethodTypeDetails`) for this specific response, focusing instead on the mandate flow.
    *   *Status:* **PARTIALLY COMPLETED & REVERTED**. The mandate flow itself is complete.

**Phase 4: Payout API Implementation**
*   **[X] Step 17: Implement `PayoutService` with payout intent creation.**
    *   *Models:* `PayoutIntentRequest.cs`, `PayoutIntentResponse.cs` with comprehensive payout data structures including bank transfer and card payout methods.
    *   *Service:* `PayoutService.cs` with `CreateAsync` method for payout intent creation.
    *   *Sample App:* Added payout test scenarios to `Program.cs` demonstrating:
        *   Bank transfer payouts with account and routing numbers
        *   Card payouts with card details
        *   Customer-associated payouts with billing information
        *   Comprehensive error handling and response formatting
    *   *Status:* **COMPLETED**.

**(Further phases for more APIs and enhancements will be added here)**
