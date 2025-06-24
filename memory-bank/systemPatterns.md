# System Patterns

## System Architecture

The overall system comprises:
1.  **`Hyperswitch.Sdk` (.NET Class Library):** Standard SDK components.
    *   Key models like `PaymentIntentResponse.cs` have been updated to include properties for top-level `error_code`, `error_message`, and a `[JsonExtensionData]` dictionary to capture any other unmapped fields from API responses. This enhances error detail visibility.
2.  **`Hyperswitch.Sdk.Sample` (.NET Console Application):** Automated test suite. Minor adjustments made to accommodate SDK model changes.
3.  **`Hyperswitch.Sdk.DemoApi` (ASP.NET Core Minimal API):**
    *   Lightweight web API for manual testing of individual SDK functions.
    *   References `Hyperswitch.Sdk`.
    *   **Dependency Injection:** `HyperswitchClient` (singleton), SDK services (scoped), `ILogger`.
    *   **API Endpoints (Direct SDK Mapping):**
        *   Exposes HTTP endpoints that directly map to individual methods on the SDK's service classes.
    *   **Request/Response Handling:**
        *   Endpoints directly use the SDK's request and response models.
        *   Returns SDK responses or error details as JSON.
    *   **Logging:**
        *   All responses (success and error) from API endpoints are serialized to JSON and logged to the console using `ILogger`.
    *   **Data Transfer Objects (DTOs):**
        *   Most custom DTOs have been removed.
    *   **Error Handling:** try-catch blocks, `Results.Problem()`.
    *   **Swagger/OpenAPI:** Enabled.

```mermaid
graph TD
    User[API Client e.g., Postman] -->|HTTP Request (SDK Model)| DemoApi[Hyperswitch.Sdk.DemoApi]
    DemoApi -->|Calls SDK Method| SdkServices[SDK Services e.g., PaymentService, CustomerService]
    SdkServices -->|Uses| HsClient[HyperswitchClient]
    HsClient -->|HTTP API Call| HyperswitchApi[Hyperswitch Platform API]

    subgraph "Project: hyperswitch-dotnet-sdk"
        DemoApi
        SdkServices
        HsClient
        ConsoleApp[Hyperswitch.Sdk.Sample] -.->|Tests| SdkServices
    end

    HyperswitchApi -.->|HTTP Response| HsClient
    HsClient -.->|SDK Response/Exception (now with enhanced error fields)| SdkServices
    SdkServices -.->|SDK Model / Exception| DemoApi
    DemoApi -- Logs Response --> ConsoleOutput[Console Output]
    DemoApi -.->|HTTP Response (SDK Model)| User
```

## Key Technical Decisions

*   **SDK Design:** Service-oriented. SDK models enhanced to capture more comprehensive error information from API responses.
*   **Demo Application Type:** ASP.NET Core Minimal API.
*   **Demo API Endpoint Strategy:** Direct 1-to-1 mapping of SDK service methods to API endpoints.
*   **Response Logging:** Implemented for all endpoints for better visibility and debugging.
*   **API Key Management:** Placeholders in `Program.cs`.
*   **Error Handling:** Centralized try-catch in Demo API; SDK models now better equipped to receive error details.

## Design Patterns in Use

*   **Singleton:** `HyperswitchClient`.
*   **Service Layer:** SDK services.
*   **Thin Wrapper (DemoApi):** The Demo API acts as a thin HTTP wrapper around the SDK services.
*   **JsonExtensionData:** Used in SDK models to capture unmapped API response fields.

## Component Relationships

*   `Hyperswitch.Sdk.DemoApi` depends on `Hyperswitch.Sdk`.
*   `Hyperswitch.Sdk.Sample` depends on `Hyperswitch.Sdk`.

## Critical Implementation Paths

*   **Authentication:** `HyperswitchClient` initialization.
*   **Serialization/Deserialization:** JSON handling of SDK models, including `JsonExtensionData` for unmapped fields and specific error properties.
*   **Error Handling:** Exception catching and logging in `DemoApi`; SDK's ability to deserialize comprehensive error responses.
*   **Endpoint Mapping:** Ensuring each relevant SDK service method has a corresponding, correctly configured API endpoint with logging.
# System Patterns

## System Architecture
- The Hyperswitch .NET SDK acts as a client library.
- It communicates with the Hyperswitch API server over HTTPS.
- It provides services (e.g., `PaymentService`, `CustomerService`) that encapsulate API interactions.
- Models are used for request and response data, ensuring type safety.

## Key Technical Decisions
- Use of `HttpClient` for making HTTP requests.
- JSON serialization/deserialization for request/response bodies.
- Asynchronous operations (`async`/`await`) for non-blocking I/O.
- Custom exception (`HyperswitchApiException`) for API-specific errors.

## Design Patterns in Use
- **Service Layer:** Services like `PaymentService`, `CustomerService`, `RefundService` abstract API endpoint interactions.
- **Model-View-Controller (Implicit):** While not a UI application, the SDK uses models for data representation, services act as controllers handling logic, and the "view" is the consuming application.
- **Factory Pattern (Potentially):** The `HyperswitchClient` could be seen as a factory or entry point for accessing various services.

## Component Relationships
- `HyperswitchClient` is the main entry point.
- `HyperswitchClient` instantiates and provides access to various services (e.g., `PaymentService`, `CustomerService`).
- Services use `HttpClient` (or a wrapper) to make API calls.
- Services use request and response models defined in `Hyperswitch.Sdk.Models`.

## Critical Implementation Paths
- **Payment Flow:**
    - Create Payment Intent (`PaymentService.CreateAsync`)
    - Confirm Payment Intent (`PaymentService.ConfirmAsync`)
    - Capture Payment (`PaymentService.CaptureAsync`)
    - Retrieve Payment Intent (`PaymentService.RetrieveAsync`)
- **Customer Management:**
    - Create Customer (`CustomerService.CreateAsync`)
    - Retrieve Customer (`CustomerService.RetrieveAsync`)
    - Update Customer (`CustomerService.UpdateAsync`)
    - List Customer Payment Methods (`CustomerService.ListPaymentMethodsAsync`)
- **Refund Flow:**
    - Create Refund (`RefundService.CreateAsync`)
    - Retrieve Refund (`RefundService.RetrieveAsync`)
