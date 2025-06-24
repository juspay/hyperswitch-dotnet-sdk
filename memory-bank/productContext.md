# Product Context

## Why This Project Exists

This project exists to provide .NET developers with a robust and easy-to-use Software Development Kit (SDK) for integrating Hyperswitch payment processing capabilities into their applications. The SDK aims to simplify interactions with the Hyperswitch API.

The `Hyperswitch.Sdk.DemoApi` sub-project specifically exists to:
*   Provide a runnable example of how to use the SDK in a backend web application context.
*   Enable manual testing and verification of various payment flows supported by the SDK.
*   Serve as a quick-start reference for developers looking to integrate the SDK.

## Problems It Solves

**For the SDK:**
*   Abstracts the complexity of direct Hyperswitch API calls.
*   Provides strongly-typed models for requests and responses, reducing integration errors.
*   Offers convenient service classes for different API functionalities (Payments, Refunds, Customers).

**For the `Hyperswitch.Sdk.DemoApi`:**
*   Demonstrates practical SDK usage in an ASP.NET Core Minimal API.
*   Allows developers to quickly see the SDK in action without building a backend from scratch.
*   Facilitates interactive testing of payment flows which can be harder to simulate in a purely automated console test suite.

## How It Should Work

**The SDK (`Hyperswitch.Sdk`):**
*   Developers instantiate `HyperswitchClient` with their API keys and profile ID.
*   Service classes (e.g., `PaymentService`, `RefundService`) are instantiated using the client.
*   Methods on these services are called with appropriate request objects (e.g., `PaymentIntentRequest`) to perform operations.
*   Methods return response objects (e.g., `PaymentIntentResponse`) or throw `HyperswitchApiException` on errors.

**The Demo API (`Hyperswitch.Sdk.DemoApi`):**
*   The API starts and listens for HTTP requests on configured endpoints.
*   Endpoints are defined in `Program.cs` using Minimal API routing.
*   Each endpoint corresponds to a specific payment-related action (e.g., create payment, capture payment, create refund).
*   When an endpoint is hit, it uses the injected SDK services (`PaymentService`, `RefundService`, etc.) to perform the action.
*   The API returns the SDK's response (or an error) as a JSON payload.
*   Users interact with these endpoints using an API client like Postman or curl.

## User Experience Goals

**For the SDK:**
*   **Ease of Integration:** Developers should find it straightforward to add the SDK to their .NET projects and start making API calls.
*   **Clarity:** API calls, request parameters, and response structures should be clear and well-documented (through code comments, and eventually, external documentation).
*   **Reliability:** The SDK should reliably handle API interactions and provide meaningful error feedback.

**For the `Hyperswitch.Sdk.DemoApi`:**
*   **Discoverability:** Endpoints should be easily discoverable (e.g., via Swagger UI).
*   **Testability:** It should be simple to send requests to the API and observe the outcomes of different payment flows.
*   **Illustrative:** The API code should clearly demonstrate best practices for using the SDK.
# Product Context: Hyperswitch .NET SDK

## Why This Project Exists
This SDK exists to enable .NET developers to seamlessly integrate Hyperswitch's payment orchestration platform into their applications. It aims to lower the barrier to entry for using Hyperswitch services within the .NET ecosystem.

## Problems It Solves
- **Integration Complexity:** Directly interacting with HTTP APIs can be time-consuming and error-prone. The SDK provides pre-built, tested, and maintained C#/.NET methods.
- **Developer Experience:** Offers .NET-idiomatic ways to work with Hyperswitch, including strongly-typed models for requests and responses, and standard exception handling.
- **Speed of Development:** Accelerates the development lifecycle for .NET applications needing payment functionalities by providing ready-to-use components.
- **Maintenance Overhead:** Reduces the burden on developers to keep up with Hyperswitch API changes, as the SDK team will manage this.

## How It Should Work
- **Intuitive API:** The SDK should expose Hyperswitch functionalities in a way that is natural for .NET developers.
- **Comprehensive Coverage:** Aim to cover all key Hyperswitch API endpoints and features relevant to typical merchant integrations.
- **Reliability:** Ensure robust error handling, clear error messages, and stable performance.
- **Security:** Adhere to best practices for handling sensitive data like API keys. Client-side sensitive data (like full card numbers) should not be unnecessarily handled or stored by the SDK itself, relying on Hyperswitch's secure mechanisms.
- **Well-Documented:** Provide clear documentation, code samples, and guides to facilitate easy adoption.
- **Testable:** The SDK should be designed in a way that allows developers to easily mock or test its interactions in their own applications.

## User Experience Goals (for the Developer using the SDK)
- **Ease of Use:** Developers should find it straightforward to install, configure, and use the SDK.
- **Clarity:** API calls, parameters, and responses should be clearly named and structured.
- **Efficiency:** Enable developers to implement payment solutions quickly and with minimal boilerplate code.
- **Confidence:** Developers should feel confident that the SDK is reliable, secure, and handles Hyperswitch interactions correctly.
