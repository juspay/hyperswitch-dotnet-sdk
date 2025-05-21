# Hyperswitch .NET Server-Side SDK Development Workflow

## 1. Objective
Generate a robust and easy-to-use .NET Server-Side SDK to interact with Hyperswitch Server-to-Server (S2S) APIs.

## 2. Core Requirements & Decisions
*   **Target Platform:** .NET 8 (LTS)
*   **Language:** C#
*   **Primary HTTP Client:** `System.Net.Http.HttpClient`
*   **JSON Serialization:** `System.Text.Json`
*   **Authentication:** API Secret Key via `Authorization: Bearer <apiKey>` header.
*   **Initial API Focus:** Payments API (Create, Retrieve).
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

**(Further phases for more APIs and enhancements will be added here)**
