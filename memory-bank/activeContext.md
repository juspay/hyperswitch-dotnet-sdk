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
# Active Context

## Current Work Focus
- Completing the implementation of the saved card payment scenario in `Hyperswitch.Sdk.Sample/Program.cs`.
- Updating Memory Bank files to reflect the completed work.

## Recent Changes
- Created `projectbrief.md`, `productContext.md`, `systemPatterns.md`, `techContext.md`, and `progress.md`.
- Added `TestSavedCardPaymentFlowAsync` method to `Hyperswitch.Sdk.Sample/Program.cs`.
- Called the new method from `Main` in `Hyperswitch.Sdk.Sample/Program.cs`.
- Corrected various model property discrepancies based on SDK definitions during implementation.
- Addressed runtime feedback:
    - Removed hardcoded `ProfileId` from payment requests in the sample scenario to allow usage of the default `ProfileId` from `HyperswitchClient`, resolving a "Business profile ... does not exist" error.
    - Added `JavaEnabled = true` to `BrowserInfo` objects in payment confirmation requests to resolve a "Missing required param: browser_info.java_enabled" error.
    - Changed `AuthenticationType` from "three_ds" to "no_three_ds" in payment intent creation for the saved card flow, as a troubleshooting step for a generic payment failure.
    - Added `CardHolderName` to `CardDetails` during the initial payment confirmation.
    - Updated `Shipping.AddressDetails` in `PaymentIntentRequest` to provide more complete address lines (Line1, Line2, Line3) and a more realistic `Line1` value ("123 Main St") to address a "Required field 'deliveryAddress.street' is not provided" error.
- Modified `Hyperswitch.Sdk/Services/CustomerService.cs`:
    - Re-added `ListPaymentMethodsForClientSecretAsync(string clientSecret)`.
    - Updated this method to use `ApiKeyType.Publishable` when calling `_client.GetAsync`, to align with the user's cURL that uses a publishable key for this endpoint.
- Updated `Hyperswitch.Sdk.Sample/Program.cs` in `TestSavedCardPaymentFlowAsync` to use this `ListPaymentMethodsForClientSecretAsync` method.
- Addressed type conflicts for `CustomerAcceptance` and `OnlineMandate`:
    - Emptied the content of `Hyperswitch.Sdk/Models/CustomerAcceptance.cs` and `Hyperswitch.Sdk/Models/OnlineMandate.cs` (which I had previously created as `OnlineAcceptanceDetails.cs`) to allow the SDK's internal definitions to be used.
    - Modified `Hyperswitch.Sdk/Models/PaymentConfirmRequest.cs` to add a `public CustomerAcceptance? CustomerAcceptance { get; set; }` property.
    - Updated `TestSavedCardPaymentFlowAsync` in `Hyperswitch.Sdk.Sample/Program.cs` to correctly instantiate `CustomerAcceptance` and its `Online` property (as `new OnlineMandate{...}`) using the SDK's presumed internal types.
- Modified `Hyperswitch.Sdk/Models/CustomerPaymentMethodListResponse.cs` to change the `JsonPropertyName` for the payment methods list from "data" to "customer_payment_methods", and added `IsGuestCustomer` property to match the API response structure when listing with `client_secret`.
- Modified `Hyperswitch.Sdk/Models/PaymentConfirmRequest.cs` again:
    - Changed `JsonPropertyName` for `PaymentMethodToken` from "payment_method_token" to "payment_token".
    - Added a top-level `string? CardCvc { get; set; }` property with `JsonPropertyName("card_cvc")`.
- Updated `TestSavedCardPaymentFlowAsync` in `Hyperswitch.Sdk.Sample/Program.cs` (Step 6 - confirming second payment with token):
    - To use the new top-level `CardCvc` property.
    - To adjust `PaymentMethodData` to only include `Billing` and remove the nested `Card` object, as CVC is now top-level and a token is used.
- Renumbered scenarios in `Hyperswitch.Sdk.Sample/Program.cs` to be sequential.
- Updated `PrintPaymentDetails` in `Hyperswitch.Sdk.Sample/Program.cs` to attempt to print connector information from `PaymentIntentResponse.ExtensionData`.

## Next Steps
- Update `progress.md` to reflect the new scenario and its corrections, including the SDK modification and scenario renumbering, and the updated print method.
- Review if `systemPatterns.md` or `techContext.md` need updates based on insights from implementing the saved card flow (e.g., usage of `PaymentMethodToken`).
- Present the completed task to the user.

## Active Decisions and Considerations
- Ensured the SDK implementation for saved cards aligns with the cURL flow provided by the user, adapting where SDK models differ from direct API call structures.
- Key SDK features used: `CustomerService.CreateCustomerAsync`, `PaymentService.CreateAsync`, `PaymentService.ConfirmPaymentAsync`, `CustomerService.ListPaymentMethodsForClientSecretAsync` (updated to use publishable key).
- Handled discrepancies between cURL parameters and available SDK model properties by prioritizing SDK definitions, including adding `CustomerAcceptance` to `PaymentConfirmRequest`.
- Used `SetupFutureUsage = "on_session"` for initial payment to enable card saving.
- Used `PaymentMethodToken` for subsequent payment confirmation.
- Updated `ListPaymentMethodsForClientSecretAsync` in `CustomerService` to explicitly use `ApiKeyType.Publishable`. This aligns the SDK call with the user's cURL that uses a publishable key for the `GET /customers/payment_methods?client_secret=...` endpoint. This should resolve the previous 422 error if the cause was an API key mismatch.
- Removed hardcoded `ProfileId` from `PaymentIntentRequest` in the sample to rely on the `HyperswitchClient`'s default profile ID, making the sample more environment-agnostic.
- Ensured `BrowserInfo.JavaEnabled` is set during payment confirmations as it's a required API parameter.
- Switched `AuthenticationType` to "no_three_ds" for the saved card sample flow to potentially simplify test card processing and avoid 3DS-related failures in a sandbox.
- Ensured `CardHolderName` is provided during the initial card payment confirmation.
- Populated `Shipping.AddressDetails` with `Line1`, `Line2`, and `Line3`, using a more realistic value for `Line1` to satisfy API validation for street address.
- Resolved type conflicts by emptying self-created `CustomerAcceptance.cs` and `OnlineMandate.cs` files, and updated sample code to use SDK's internal `OnlineMandate` type for `CustomerAcceptance.Online` property.
- Corrected deserialization of `CustomerPaymentMethodListResponse` by aligning `JsonPropertyName` with the actual API response field ("customer_payment_methods") and adding missing fields like `IsGuestCustomer`.
- Aligned `PaymentConfirmRequest` model with cURL for tokenized payments:
    - Changed `PaymentMethodToken` to serialize as `payment_token`.
    - Added top-level `CardCvc` (serializing as `card_cvc`).
    - Ensured `PaymentMethodData` only contains `Billing` when confirming with a token, to avoid "missing card_number" errors if a partial `card` object was previously included.
- Maintained sequential numbering for test scenarios in the sample program for clarity.
- Utilized `JsonExtensionData` in `PaymentIntentResponse` to access and display fields not explicitly mapped in the model, such as connector information.

## Important Patterns and Preferences
- Adherence to the Memory Bank structure and update workflows is critical.

## Learnings and Project Insights
- Initializing the Memory Bank is the first priority.
- Careful inspection of SDK model definitions (`PaymentIntentRequest`, `PaymentConfirmRequest`, `CardDetails`, `Address`, etc.) is crucial to correctly map API concepts to SDK usage. Discrepancies between direct API call structures (like cURLs) and SDK models are common and require adaptation.
- The `ListPaymentMethodsForClientSecretAsync` method in `CustomerService` now explicitly requests the use of the `PublishableKey` via `_client.GetAsync(requestUrl, ApiKeyType.Publishable)`. This leverages the SDK's capability (discovered by reading `HyperswitchClient.cs`) to switch API key types for requests.
- Confirming payments with a saved token (`PaymentMethodToken`) requires careful construction of `PaymentConfirmRequest`, potentially including CVC in `PaymentMethodData.Card.CardCvc`.
- Hardcoding profile IDs from external examples (like cURLs) into SDK sample code can lead to environment-specific errors. Samples should rely on configurable defaults (like `HyperswitchClient.DefaultProfileId`) where possible.
- Iteratively resolved type conflicts by creating, then emptying, model files (`CustomerAcceptance.cs`, `OnlineMandate.cs`) to allow pre-existing SDK definitions to take precedence, and then adjusting sample code to use the correct (inferred) SDK types (e.g., `OnlineMandate` for `CustomerAcceptance.Online`).
- API error messages like "Missing required param" are key indicators for adjusting SDK model population, even if the parameter wasn't obvious from initial cURL examples or SDK model property names alone (e.g. `JavaEnabled` vs `JavaScriptEnabled`).
- Generic payment failures (status: "failed" without specific error codes in `LastPaymentError`) can be due to various reasons including test card limitations, connector configurations, or subtle missing data like `CardHolderName` or an unsuitable `AuthenticationType` for the test environment.
- Errors like "Required field 'deliveryAddress.street' is not provided" highlight the importance of providing complete and valid-looking address data, even for test scenarios, as some gateways perform validation on these fields. Placeholder values like "sdsdfsdf" might be rejected.
- Mismatches between SDK model `JsonPropertyName` attributes and actual API response field names are a common cause of deserialization failures (e.g., list property being null when data is present under a different key).
- For tokenized payments, the structure of `PaymentConfirmRequest` can differ significantly from new card payments. Fields like `card_cvc` might be top-level, and `payment_method_data.card` might need to be omitted to prevent deserialization errors on the server if it expects a full card object when the `card` key is present. The token name itself (`payment_token` vs. `payment_method_token`) is also critical.
