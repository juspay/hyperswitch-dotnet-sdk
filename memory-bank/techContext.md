# Tech Context

## Technologies Used

*   **.NET 6+:** The SDK, Sample Console App, and Demo API are built using .NET. (Assuming .NET 6+ for Minimal APIs, can be adjusted if a different version is confirmed for the SDK).
*   **C#:** Primary programming language.
*   **ASP.NET Core:** Used for the `Hyperswitch.Sdk.DemoApi` (specifically Minimal API features).
*   **System.Net.Http.HttpClient:** Used by the SDK for making HTTP requests to the Hyperswitch API.
*   **System.Text.Json (or Newtonsoft.Json):** Used for JSON serialization and deserialization within the SDK. (The specific library should be confirmed by inspecting SDK's dependencies or code).
*   **Swagger/OpenAPI:** Integrated into the `Hyperswitch.Sdk.DemoApi` for API documentation and interactive testing via Swagger UI.

## Development Setup

*   **.NET SDK (latest version recommended):** Required to build and run the projects.
*   **IDE:** Visual Studio, VS Code, or JetBrains Rider.
*   **API Client (for DemoApi):** Postman, Insomnia, curl, or similar tool for sending HTTP requests to the demo API endpoints.
*   **Hyperswitch Account & API Keys:** Necessary for the SDK to authenticate and interact with the Hyperswitch platform. These keys need to be configured (currently hardcoded placeholders in `Hyperswitch.Sdk.DemoApi/Program.cs` and `Hyperswitch.Sdk.Sample/Program.cs`).

**To run the `Hyperswitch.Sdk.DemoApi`:**
1.  Ensure API keys are correctly set in `Hyperswitch.Sdk.DemoApi/Program.cs` (or configured via a proper mechanism).
2.  Navigate to the `Hyperswitch.Sdk.DemoApi` directory in a terminal.
3.  Run `dotnet run`.
4.  The API will typically be available at `https://localhost:<port>` and `http://localhost:<port>`. Swagger UI will be at `https://localhost:<port>/swagger`.

## Technical Constraints

*   **Network Connectivity:** The SDK and Demo API require internet access to communicate with the Hyperswitch API.
*   **API Rate Limits:** Calls to the Hyperswitch API may be subject to rate limits imposed by the platform. The SDK does not currently implement automatic retry logic for rate limiting.
*   **Security:** API secret keys must be handled securely and should not be hardcoded in production applications. The current hardcoding in the demo API is for ease of local testing only.

## Dependencies

**`Hyperswitch.Sdk`:**
*   Standard .NET libraries.
*   Likely `System.Text.Json` or `Newtonsoft.Json` for JSON processing.

**`Hyperswitch.Sdk.Sample`:**
*   `Hyperswitch.Sdk` (project reference).

**`Hyperswitch.Sdk.DemoApi`:**
*   `Hyperswitch.Sdk` (project reference).
*   `Swashbuckle.AspNetCore` (version 9.0.1 or latest, for Swagger/OpenAPI).

## Tool Usage Patterns

*   **`dotnet CLI`:** Used for creating new projects (`dotnet new`), adding references (`dotnet add reference`), building (`dotnet build`), and running (`dotnet run`) the applications.
*   **Git:** For version control.
*   **IDE Debuggers:** For stepping through code and troubleshooting issues in the SDK, Sample App, or Demo API.
*   **API Clients (Postman, etc.):** Essential for manually testing the `Hyperswitch.Sdk.DemoApi` endpoints by crafting and sending HTTP requests.
