# Active Context

## Current Work Focus

Ensuring error details (like `error_message`) from Hyperswitch are correctly captured by the SDK and made visible through the Demo API's logging.

Current sub-task:
1.  Updated `Hyperswitch.Sdk/Models/PaymentIntentResponse.cs` to include `ErrorCode`, `ErrorMessage`, and `ExtensionData` properties.
2.  Applied a speculative fix to `Hyperswitch.Sdk.Sample/Program.cs` for a build error related to `NextAction.RedirectToUrl` by making the null check more explicit.
3.  The Demo API (`Hyperswitch.Sdk.DemoApi/Program.cs`) already has response logging implemented.

## Recent Changes

*   Modified `Hyperswitch.Sdk/Models/PaymentIntentResponse.cs`:
    *   Added `string? ErrorCode { get; set; }` with `[JsonPropertyName("error_code")]`.
    *   Added `string? ErrorMessage { get; set; }` with `[JsonPropertyName("error_message")]`.
    *   Added `Dictionary<string, object>? ExtensionData { get; set; }` with `[JsonExtensionData]`.
*   Modified `Hyperswitch.Sdk.Sample/Program.cs`:
    *   Changed `paymentIntent.NextAction?.RedirectToUrl` to `(paymentIntent.NextAction != null ? paymentIntent.NextAction.RedirectToUrl : "N/A")` to potentially resolve a build error.
*   `Hyperswitch.Sdk.DemoApi/Program.cs` has response logging implemented.

## Next Steps

1.  Update `memory-bank/progress.md` and `memory-bank/systemPatterns.md` to reflect these SDK and Sample app changes.
2.  User to **rebuild the entire solution** (`dotnet build server-sdk.sln`) to ensure changes in the SDK project are picked up by dependent projects.
3.  User to run the Demo API (`Hyperswitch.Sdk.DemoApi`) and re-test the failing payment cURL.
4.  User to observe the console logs from the Demo API to see if `ErrorCode`, `ErrorMessage`, or `ExtensionData` in the logged `PaymentIntentResponse` are populated for the failed payment.
5.  Based on the logs, determine if further SDK model adjustments are needed or if the current setup captures all necessary error information.

## Active Decisions and Considerations

*   **Error Message Handling:** Prioritizing capturing error messages from Hyperswitch. The current changes to `PaymentIntentResponse.cs` aim to catch both explicitly defined top-level error fields (as per OpenAPI spec) and any other undefined fields via `ExtensionData`.
*   **Build Issues:** Addressing build errors as they arise, sometimes requiring alternative syntax if compiler behavior is unexpected with certain null-conditional accesses.

## Important Patterns and Preferences

*   Modifying SDK models directly is the preferred way to ensure all API response data is captured.
*   Console logging in the Demo API is a key tool for verifying SDK behavior.

## Learnings and Project Insights

*   Build errors can sometimes be misleading if the underlying type definitions are correct; explicit coding patterns or clean builds can sometimes resolve them.
*   The OpenAPI specification is the definitive source for expected API response structures.
