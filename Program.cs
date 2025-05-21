using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperswitch.Sdk;
using Hyperswitch.Sdk.Models;
using Hyperswitch.Sdk.Services;
using Hyperswitch.Sdk.Exceptions;

namespace Hyperswitch.Sdk.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hyperswitch SDK Sample");
            string apiKey = "snd_EOQGADPyoizCvcr733PMNPX1GfO5UyalBwqMRGki8RqMyEMc7BtD2sSSFEUMCnJQ"; 
            var client = new HyperswitchClient(apiKey);
            var paymentService = new PaymentService(client);

            // Scenario 1: Manual Capture (Two-Step: Create then Confirm)
            Console.WriteLine("\n--- SCENARIO 1: MANUAL CAPTURE (Two-Step Create-Confirm) ---");
            await TestPaymentFlowTwoStep(paymentService, captureMethod: "manual", apiKey: apiKey);

            // Scenario 2: Automatic Capture (Two-Step: Create then Confirm)
            Console.WriteLine("\n--- SCENARIO 2: AUTOMATIC CAPTURE (Two-Step Create-Confirm) ---");
            await TestPaymentFlowTwoStep(paymentService, captureMethod: "automatic", apiKey: apiKey);
            
            // Scenario 3: Single-Call Manual Capture (Create with Confirm:true)
            Console.WriteLine("\n--- SCENARIO 3: SINGLE-CALL MANUAL CAPTURE (Create with Confirm:true) ---");
            await TestSingleCallManualCaptureAsync(paymentService, apiKey: apiKey);

            client.Dispose();
        }

        // Renamed from TestPaymentFlow to TestPaymentFlowTwoStep for clarity
        static async Task TestPaymentFlowTwoStep(PaymentService paymentService, string captureMethod, string apiKey)
        {
            Console.WriteLine($"Testing Two-Step Flow with CaptureMethod: {captureMethod}");
            PaymentIntentResponse? paymentIntent = null;

            try
            {
                // Step 1: Create Payment Intent (confirm: false)
                Console.WriteLine("\n1. Creating Payment Intent (confirm: false)...");
                var createRequest = new PaymentIntentRequest
                {
                    Amount = 650, 
                    Currency = "USD",
                    ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn", 
                    Confirm = false, 
                    CaptureMethod = captureMethod,
                    PaymentMethod = "card", 
                    PaymentMethodType = "credit", 
                    Email = "customer-sdk-test@example.com",
                    Description = $"Test SDK Payment ({captureMethod} capture, two-step)",
                    ReturnUrl = "https://example.com/sdk_return", 
                    PaymentMethodData = new PaymentMethodData
                    {
                        Card = new CardDetails { CardNumber = "4917610000000000", CardExpiryMonth = "03", CardExpiryYear = "2030", CardCvc = "737" },
                        Billing = new Address { AddressDetails = new AddressDetails { Line1 = "123 Test St", City="Testville", Country="US", Zip="12345"} }
                    },
                    AuthenticationType = "no_three_ds", 
                    BrowserInfo = new BrowserInfo { ColorDepth = 24, JavaEnabled = true, JavaScriptEnabled = true, Language = "en-GB", ScreenHeight = 720, ScreenWidth = 1280, TimeZone = -330, IpAddress = "208.127.127.193", AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8", UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.0.0" }
                };
                paymentIntent = await paymentService.CreateAsync(createRequest);
                if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.PaymentId) || string.IsNullOrEmpty(paymentIntent.ClientSecret)) { PrintAndReturnError("PaymentId or ClientSecret is missing after create."); return; }
                PrintPaymentDetails("1. After Create (confirm:false)", paymentIntent);

                // Step 2: Confirm Payment
                if (paymentIntent.Status == "requires_confirmation" || paymentIntent.Status == "requires_payment_method")
                {
                    Console.WriteLine($"\n2. Confirming Payment Intent {paymentIntent.PaymentId}...");
                    var confirmRequest = new PaymentConfirmRequest { ReturnUrl = createRequest.ReturnUrl, BrowserInfo = createRequest.BrowserInfo, CaptureMethod = createRequest.CaptureMethod };
                    paymentIntent = await paymentService.ConfirmPaymentAsync(paymentIntent.PaymentId!, confirmRequest);
                    if (paymentIntent == null) { PrintAndReturnError("Payment Intent confirmation returned null or failed."); return; }
                    PrintPaymentDetails("2. After Confirm", paymentIntent);
                } else { Console.WriteLine($"\n2. Payment status is '{paymentIntent.Status}', skipping explicit Confirm call."); }

                // Step 3: Sync Status
                Console.WriteLine($"\n3. Syncing status for {paymentIntent.PaymentId}...");
                paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                if (paymentIntent == null) { PrintAndReturnError("Payment Intent sync returned null or failed."); return; }
                PrintPaymentDetails("3. After Sync", paymentIntent);

                // Step 4: Capture Payment (only if manual capture and status is requires_capture)
                if (captureMethod == "manual" && paymentIntent.Status == "requires_capture" && !string.IsNullOrEmpty(paymentIntent.PaymentId))
                {
                    Console.WriteLine($"\n4. Capturing Payment {paymentIntent.PaymentId}...");
                    var captureRequest = new PaymentCaptureRequest { AmountToCapture = paymentIntent.AmountCapturable };
                    paymentIntent = await paymentService.CapturePaymentAsync(paymentIntent.PaymentId!, captureRequest);
                    if (paymentIntent == null) { PrintAndReturnError("Payment Capture returned null or failed."); return; }
                    PrintPaymentDetails("4. After Capture", paymentIntent);

                    Console.WriteLine($"\n5. Final Sync for {paymentIntent.PaymentId} after capture attempt...");
                    paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                    PrintPaymentDetails("5. After Final Sync (post-capture)", paymentIntent);
                } else if (captureMethod == "manual") { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n4. Payment status is '{paymentIntent.Status}' (expected 'requires_capture' for manual test). Capture not attempted."); Console.ResetColor();
                } else { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n4. Automatic capture method. Final status should be checked after confirm/sync."); Console.ResetColor(); }
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, $"in {captureMethod} two-step flow"); }
            catch (Exception ex) { PrintGenericError(ex, $"in {captureMethod} two-step flow"); }
        }

        static async Task TestSingleCallManualCaptureAsync(PaymentService paymentService, string apiKey)
        {
            Console.WriteLine($"Testing Single-Call Flow with Confirm:true, CaptureMethod:manual");
            PaymentIntentResponse? paymentIntent = null;
            try
            {
                // Step 1: Create Payment Intent (confirm: true, capture_method: manual)
                Console.WriteLine("\n1. Creating Payment Intent (confirm: true, capture_method: manual)...");
                var createRequest = new PaymentIntentRequest
                {
                    Amount = 750, 
                    Currency = "USD",
                    ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn",
                    Confirm = true, 
                    CaptureMethod = "manual",
                    PaymentMethod = "card",
                    PaymentMethodType = "credit",
                    Email = "customer-single-call-manual@example.com",
                    Description = "Test SDK Payment (Single-Call Manual Capture)",
                    ReturnUrl = "https://example.com/sdk_single_call_return",
                    PaymentMethodData = new PaymentMethodData
                    {
                        Card = new CardDetails { CardNumber = "4917610000000000", CardExpiryMonth = "03", CardExpiryYear = "2030", CardCvc = "737" },
                        Billing = new Address { AddressDetails = new AddressDetails { Line1 = "789 Test Ave", City="Testburg", Country="US", Zip="67890"} }
                    },
                    AuthenticationType = "no_three_ds",
                    BrowserInfo = new BrowserInfo { ColorDepth = 24, JavaEnabled = true, JavaScriptEnabled = true, Language = "en-US", ScreenHeight = 1080, ScreenWidth = 1920, TimeZone = -300, IpAddress = "208.127.127.194", AcceptHeader = "application/json", UserAgent = "Mozilla/5.0 SDK Test" }
                };
                paymentIntent = await paymentService.CreateAsync(createRequest);
                if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.PaymentId)) { PrintAndReturnError("PaymentId is missing after create."); return; }
                PrintPaymentDetails("1. After Create (confirm:true, manual capture)", paymentIntent);

                // Step 2: Sync Status (good practice after action)
                Console.WriteLine($"\n2. Syncing status for {paymentIntent.PaymentId}...");
                paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                if (paymentIntent == null) { PrintAndReturnError("Payment Intent sync returned null or failed."); return; }
                PrintPaymentDetails("2. After Sync", paymentIntent);

                // Step 3: Capture Payment (if status is requires_capture)
                if (paymentIntent.Status == "requires_capture" && !string.IsNullOrEmpty(paymentIntent.PaymentId))
                {
                    Console.WriteLine($"\n3. Capturing Payment {paymentIntent.PaymentId}...");
                    var captureRequest = new PaymentCaptureRequest { AmountToCapture = paymentIntent.AmountCapturable };
                    paymentIntent = await paymentService.CapturePaymentAsync(paymentIntent.PaymentId!, captureRequest);
                    if (paymentIntent == null) { PrintAndReturnError("Payment Capture returned null or failed."); return; }
                    PrintPaymentDetails("3. After Capture", paymentIntent);

                    Console.WriteLine($"\n4. Final Sync for {paymentIntent.PaymentId} after capture attempt...");
                    paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                    PrintPaymentDetails("4. After Final Sync (post-capture)", paymentIntent);
                } else { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n3. Payment status is '{paymentIntent.Status}' (expected 'requires_capture'). Capture not attempted."); Console.ResetColor(); }
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in single-call manual capture flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in single-call manual capture flow"); }
        }

        static void PrintPaymentDetails(string stage, PaymentIntentResponse? payment)
        {
            if (payment == null) { PrintAndReturnError($"{stage}: Payment details are null."); return; }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{stage}:");
            Console.WriteLine($"  ID: {payment.PaymentId}, Status: {payment.Status}");
            Console.WriteLine($"  Amount: {payment.Amount} {payment.Currency}, Capturable: {payment.AmountCapturable}, Received: {payment.AmountReceived}");
            Console.WriteLine($"  Client Secret: {payment.ClientSecret}");
            if (!string.IsNullOrEmpty(payment.NextAction?.RedirectToUrl)) { Console.WriteLine($"  Next Action URL: {payment.NextAction.RedirectToUrl}"); }
            if (payment.LastPaymentError != null) { Console.WriteLine($"  Last Error: [{payment.LastPaymentError.Code}] {payment.LastPaymentError.Message}"); }
            Console.ResetColor();
        }

        static void PrintAndReturnError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        static void PrintApiError(HyperswitchApiException apiEx, string flowContext)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Hyperswitch API Error {flowContext}:");
            Console.WriteLine($"  Status Code: {apiEx.StatusCode}");
            Console.WriteLine($"  Message: {apiEx.Message}");
            if (apiEx.ErrorDetails?.Error != null) { Console.WriteLine($"  Error Code: {apiEx.ErrorDetails.Error.Code}, Msg: {apiEx.ErrorDetails.Error.Message}"); }
            Console.WriteLine($"  Raw Response: {apiEx.ResponseContent}");
            Console.ResetColor();
        }
        static void PrintGenericError(Exception ex, string flowContext)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An unexpected error occurred {flowContext}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
        }
    }
}
