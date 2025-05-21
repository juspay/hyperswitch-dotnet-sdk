using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("Hyperswitch SDK Sample - Full Test Suite");
            string apiKey = "snd_EOQGADPyoizCvcr733PMNPX1GfO5UyalBwqMRGki8RqMyEMc7BtD2sSSFEUMCnJQ"; 
            var client = new HyperswitchClient(apiKey);
            var paymentService = new PaymentService(client);
            var refundService = new RefundService(client); 
            var customerService = new CustomerService(client);

            var createdPaymentIds = new List<string?>();
            RefundResponse? lastCreatedRefund = null;
            string? testCustomerId = "cus_y3h0zEXxP9Z2rP9cM0xZ"; // From your cURL example

            // Scenario 1: Manual Capture (Two-Step: Create then Confirm)
            Console.WriteLine("\n--- SCENARIO 1: MANUAL CAPTURE (Two-Step Create-Confirm) ---");
            var p1 = await TestPaymentFlowTwoStep(paymentService, "manual");
            if(p1 != null && !string.IsNullOrEmpty(p1.PaymentId)) createdPaymentIds.Add(p1.PaymentId);

            // Scenario 2: Automatic Capture (Two-Step: Create then Confirm)
            Console.WriteLine("\n--- SCENARIO 2: AUTOMATIC CAPTURE (Two-Step Create-Confirm) ---");
            var p2 = await TestPaymentFlowTwoStep(paymentService, "automatic");
            if(p2 != null && !string.IsNullOrEmpty(p2.PaymentId)) createdPaymentIds.Add(p2.PaymentId);
            
            // Scenario 3: Single-Call Manual Capture (Create with Confirm:true)
            Console.WriteLine("\n--- SCENARIO 3: SINGLE-CALL MANUAL CAPTURE (Create with Confirm:true) ---");
            var p3 = await TestSingleCallManualCaptureAsync(paymentService);
            if(p3 != null && !string.IsNullOrEmpty(p3.PaymentId)) createdPaymentIds.Add(p3.PaymentId);
            
            // Scenario 4: Void/Cancel Payment
            Console.WriteLine("\n--- SCENARIO 4: VOID/CANCEL PAYMENT ---");
            var p4 = await TestVoidPaymentAsync(paymentService);
            if(p4 != null && !string.IsNullOrEmpty(p4.PaymentId)) createdPaymentIds.Add(p4.PaymentId);

            // Scenario 5: Update Payment
            Console.WriteLine("\n--- SCENARIO 5: UPDATE PAYMENT ---");
            var p5 = await TestUpdatePaymentAsync(paymentService);
            if(p5 != null && !string.IsNullOrEmpty(p5.PaymentId)) createdPaymentIds.Add(p5.PaymentId);

            // Scenario 6: Refund Payment (and then use this refund for Update Refund test)
            Console.WriteLine("\n--- SCENARIO 6: REFUND PAYMENT ---");
            var paymentForRefund = await CreateAndConfirmPaymentForRefund(paymentService); 
            if (paymentForRefund != null && paymentForRefund.Status == "succeeded" && !string.IsNullOrEmpty(paymentForRefund.PaymentId))
            {
                 lastCreatedRefund = await TestRefundPaymentAsync(paymentService, refundService, paymentForRefund.PaymentId);
            }
            else
            {
                Console.WriteLine($"Skipping Refund test as prerequisite payment was not successful (Status: {paymentForRefund?.Status}). Manual intervention (browser auth) might be needed for the payment to succeed.");
            }

            // Scenario 7: Update Refund (uses refund from Scenario 6 if successful)
            Console.WriteLine("\n--- SCENARIO 7: UPDATE REFUND ---");
            if (lastCreatedRefund != null && !string.IsNullOrEmpty(lastCreatedRefund.RefundId) && lastCreatedRefund.Status == "pending") 
            {
                await TestUpdateRefundAsync(refundService, lastCreatedRefund.RefundId);
            }
            else
            {
                Console.WriteLine($"Skipping Update Refund test as no suitable 'pending' refund was created/retrieved in Scenario 6 (Refund Status: {lastCreatedRefund?.Status}).");
            }

            // Scenario 8: List Refunds
            Console.WriteLine("\n--- SCENARIO 8: LIST REFUNDS ---");
            string? paymentIdForListTest = null; 
            if(lastCreatedRefund?.PaymentId != null) 
            {
                paymentIdForListTest = lastCreatedRefund.PaymentId; 
                Console.WriteLine($"Listing refunds for payment ID: {paymentIdForListTest} (from Scenario 6 refund)");
                await TestListRefundsAsync(refundService, paymentIdToList: paymentIdForListTest); 
            }
            else if (createdPaymentIds.Any(id => !string.IsNullOrEmpty(id)))
            {
                paymentIdForListTest = createdPaymentIds.First(id => !string.IsNullOrEmpty(id));
                 Console.WriteLine($"Listing refunds for payment ID: {paymentIdForListTest} (from earlier scenarios)");
                await TestListRefundsAsync(refundService, paymentIdToList: paymentIdForListTest);
            }
            Console.WriteLine("Listing general refunds (limit 3)...");
            await TestListRefundsAsync(refundService, limit: 3); 

            // Scenario 9: List Customer Payment Methods
            Console.WriteLine("\n--- SCENARIO 9: LIST CUSTOMER PAYMENT METHODS ---");
            await TestListCustomerPaymentMethodsAsync(customerService, testCustomerId);

            client.Dispose();
        }
        
        static async Task<PaymentIntentResponse?> CreateAndConfirmPaymentForRefund(PaymentService paymentService)
        {
            Console.WriteLine("   Creating a payment with auto-capture for refund testing...");
            var createRequest = new PaymentIntentRequest { Amount = 1200, Currency = "USD", ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn", Confirm = true, CaptureMethod = "automatic", PaymentMethod = "card", PaymentMethodType = "credit", Email = "customer-for-refund@example.com", Description = "Payment for Refund Test", ReturnUrl = "https://example.com/sdk_auto_capture_return", PaymentMethodData = new PaymentMethodData { Card = new CardDetails { CardNumber = "4917610000000000", CardExpiryMonth = "03", CardExpiryYear = "2030", CardCvc = "737" }, Billing = new Address { AddressDetails = new AddressDetails { Line1 = "404 Refund Ln", City="SucceedCity", Country="US", Zip="33333"} } }, AuthenticationType = "no_three_ds", BrowserInfo = new BrowserInfo { ColorDepth = 24, JavaEnabled = true, JavaScriptEnabled = true, Language = "en-US", ScreenHeight = 720, ScreenWidth = 1280, TimeZone = -330, IpAddress = "208.127.127.198", AcceptHeader = "application/json", UserAgent = "Mozilla/5.0 SDK Refund Prereq" } };
            PaymentIntentResponse? paymentIntent = await paymentService.CreateAsync(createRequest);
            if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.PaymentId)) { PrintAndReturnError("   Payment creation for refund test failed."); return null; }
            PrintPaymentDetails("   Prereq Payment Created", paymentIntent);

            int attempts = 0;
            while (paymentIntent.Status == "requires_customer_action" || paymentIntent.Status == "processing" || paymentIntent.Status == "requires_capture")
            {
                if (attempts >= 3) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"   Payment {paymentIntent.PaymentId} did not reach 'succeeded' after {attempts} syncs for refund test. Current status: {paymentIntent.Status}. Manual browser auth might be needed for this payment to be used in refund tests."); Console.ResetColor(); return paymentIntent; }
                Console.WriteLine($"\n   Syncing prereq payment {paymentIntent.PaymentId} (attempt {++attempts}), current status: {paymentIntent.Status}...");
                await Task.Delay(2000); 
                paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                if (paymentIntent == null) { PrintAndReturnError("   Sync returned null for refund prereq."); return null; }
                PrintPaymentDetails($"   After Prereq Sync Attempt {attempts}", paymentIntent);
            }
            return paymentIntent;
        }

        static async Task<PaymentIntentResponse?> TestPaymentFlowTwoStep(PaymentService paymentService, string captureMethod)
        {
            Console.WriteLine($"Testing Two-Step Flow with CaptureMethod: {captureMethod}");
            PaymentIntentResponse? paymentIntent = null;
            try
            {
                Console.WriteLine("\n1. Creating Payment Intent (confirm: false)...");
                var createRequest = new PaymentIntentRequest { Amount = 650, Currency = "USD", ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn", Confirm = false, CaptureMethod = captureMethod, PaymentMethod = "card", PaymentMethodType = "credit", Email = "customer-sdk-test@example.com", Description = $"Test SDK Payment ({captureMethod} capture, two-step)", ReturnUrl = "https://example.com/sdk_return", PaymentMethodData = new PaymentMethodData { Card = new CardDetails { CardNumber = "4917610000000000", CardExpiryMonth = "03", CardExpiryYear = "2030", CardCvc = "737" }, Billing = new Address { AddressDetails = new AddressDetails { Line1 = "123 Test St", City="Testville", Country="US", Zip="12345"} } }, AuthenticationType = "no_three_ds", BrowserInfo = new BrowserInfo { ColorDepth = 24, JavaEnabled = true, JavaScriptEnabled = true, Language = "en-GB", ScreenHeight = 720, ScreenWidth = 1280, TimeZone = -330, IpAddress = "208.127.127.193", AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8", UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.0.0" } };
                paymentIntent = await paymentService.CreateAsync(createRequest);
                if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.PaymentId) || string.IsNullOrEmpty(paymentIntent.ClientSecret)) { PrintAndReturnError("PaymentId or ClientSecret is missing after create."); return null; }
                PrintPaymentDetails("1. After Create (confirm:false)", paymentIntent);

                if (paymentIntent.Status == "requires_confirmation" || paymentIntent.Status == "requires_payment_method")
                {
                    Console.WriteLine($"\n2. Confirming Payment Intent {paymentIntent.PaymentId}...");
                    var confirmRequest = new PaymentConfirmRequest { ReturnUrl = createRequest.ReturnUrl, BrowserInfo = createRequest.BrowserInfo, CaptureMethod = createRequest.CaptureMethod };
                    paymentIntent = await paymentService.ConfirmPaymentAsync(paymentIntent.PaymentId!, confirmRequest);
                    if (paymentIntent == null) { PrintAndReturnError("Payment Intent confirmation returned null or failed."); return null; }
                    PrintPaymentDetails("2. After Confirm", paymentIntent);
                } else { Console.WriteLine($"\n2. Payment status is '{paymentIntent.Status}', skipping explicit Confirm call."); }

                Console.WriteLine($"\n3. Syncing status for {paymentIntent.PaymentId}...");
                paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                if (paymentIntent == null) { PrintAndReturnError("Payment Intent sync returned null or failed."); return null; }
                PrintPaymentDetails("3. After Sync", paymentIntent);

                if (captureMethod == "manual" && paymentIntent.Status == "requires_capture" && !string.IsNullOrEmpty(paymentIntent.PaymentId))
                {
                    Console.WriteLine($"\n4. Capturing Payment {paymentIntent.PaymentId}...");
                    var captureRequest = new PaymentCaptureRequest { AmountToCapture = paymentIntent.AmountCapturable };
                    paymentIntent = await paymentService.CapturePaymentAsync(paymentIntent.PaymentId!, captureRequest);
                    if (paymentIntent == null) { PrintAndReturnError("Payment Capture returned null or failed."); return null; }
                    PrintPaymentDetails("4. After Capture", paymentIntent);
                    Console.WriteLine($"\n5. Final Sync for {paymentIntent.PaymentId} after capture attempt...");
                    paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                    PrintPaymentDetails("5. After Final Sync (post-capture)", paymentIntent);
                } else if (captureMethod == "manual") { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n4. Payment status is '{paymentIntent.Status}' (expected 'requires_capture' for manual test). Capture not attempted."); Console.ResetColor();
                } else { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n4. Automatic capture method. Final status should be checked after confirm/sync."); Console.ResetColor(); }
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, $"in {captureMethod} two-step flow"); }
            catch (Exception ex) { PrintGenericError(ex, $"in {captureMethod} two-step flow"); }
            return paymentIntent;
        }

        static async Task<PaymentIntentResponse?> TestSingleCallManualCaptureAsync(PaymentService paymentService)
        {
            Console.WriteLine($"Testing Single-Call Flow with Confirm:true, CaptureMethod:manual");
            PaymentIntentResponse? paymentIntent = null;
            try
            {
                Console.WriteLine("\n1. Creating Payment Intent (confirm: true, capture_method: manual)...");
                var createRequest = new PaymentIntentRequest { Amount = 750, Currency = "USD", ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn", Confirm = true, CaptureMethod = "manual", PaymentMethod = "card", PaymentMethodType = "credit", Email = "customer-single-call-manual@example.com", Description = "Test SDK Payment (Single-Call Manual Capture)", ReturnUrl = "https://example.com/sdk_single_call_return", PaymentMethodData = new PaymentMethodData { Card = new CardDetails { CardNumber = "4917610000000000", CardExpiryMonth = "03", CardExpiryYear = "2030", CardCvc = "737" }, Billing = new Address { AddressDetails = new AddressDetails { Line1 = "789 Test Ave", City="Testburg", Country="US", Zip="67890"} } }, AuthenticationType = "no_three_ds", BrowserInfo = new BrowserInfo { ColorDepth = 24, JavaEnabled = true, JavaScriptEnabled = true, Language = "en-US", ScreenHeight = 1080, ScreenWidth = 1920, TimeZone = -300, IpAddress = "208.127.127.194", AcceptHeader = "application/json", UserAgent = "Mozilla/5.0 SDK Test" } };
                paymentIntent = await paymentService.CreateAsync(createRequest);
                if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.PaymentId)) { PrintAndReturnError("PaymentId is missing after create."); return null; }
                PrintPaymentDetails("1. After Create (confirm:true, manual capture)", paymentIntent);

                Console.WriteLine($"\n2. Syncing status for {paymentIntent.PaymentId}...");
                paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                if (paymentIntent == null) { PrintAndReturnError("Payment Intent sync returned null or failed."); return null; }
                PrintPaymentDetails("2. After Sync", paymentIntent);

                if (paymentIntent.Status == "requires_capture" && !string.IsNullOrEmpty(paymentIntent.PaymentId))
                {
                    Console.WriteLine($"\n3. Capturing Payment {paymentIntent.PaymentId}...");
                    var captureRequest = new PaymentCaptureRequest { AmountToCapture = paymentIntent.AmountCapturable };
                    paymentIntent = await paymentService.CapturePaymentAsync(paymentIntent.PaymentId!, captureRequest);
                    if (paymentIntent == null) { PrintAndReturnError("Payment Capture returned null or failed."); return null; }
                    PrintPaymentDetails("3. After Capture", paymentIntent);
                    Console.WriteLine($"\n4. Final Sync for {paymentIntent.PaymentId} after capture attempt...");
                    paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                    PrintPaymentDetails("4. After Final Sync (post-capture)", paymentIntent);
                } else { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n3. Payment status is '{paymentIntent.Status}' (expected 'requires_capture'). Capture not attempted."); Console.ResetColor(); }
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in single-call manual capture flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in single-call manual capture flow"); }
            return paymentIntent;
        }

        static async Task<PaymentIntentResponse?> TestVoidPaymentAsync(PaymentService paymentService)
        {
            Console.WriteLine($"Testing Void/Cancel Payment Flow");
            PaymentIntentResponse? paymentIntent = null;
            try
            {
                Console.WriteLine("\n1. Creating Payment Intent for Void Test (confirm: true, capture_method: manual)...");
                var createRequest = new PaymentIntentRequest { Amount = 800, Currency = "USD", ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn", Confirm = true, CaptureMethod = "manual", PaymentMethod = "card", PaymentMethodType = "credit", Email = "customer-void-test@example.com", Description = "Test SDK Payment (for Voiding)", ReturnUrl = "https://example.com/sdk_void_return", PaymentMethodData = new PaymentMethodData { Card = new CardDetails { CardNumber = "4917610000000000", CardExpiryMonth = "03", CardExpiryYear = "2030", CardCvc = "737" }, Billing = new Address { AddressDetails = new AddressDetails { Line1 = "101 Void St", City="Cancelburg", Country="US", Zip="00000"} } }, AuthenticationType = "no_three_ds", BrowserInfo = new BrowserInfo { ColorDepth = 24, JavaEnabled = true, JavaScriptEnabled = true, Language = "en-US", ScreenHeight = 720, ScreenWidth = 1280, TimeZone = -330, IpAddress = "208.127.127.195", AcceptHeader = "application/json", UserAgent = "Mozilla/5.0 SDK Void Test" } };
                paymentIntent = await paymentService.CreateAsync(createRequest);
                if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.PaymentId)) { PrintAndReturnError("PaymentId is missing after create for void test."); return null; }
                PrintPaymentDetails("1. After Create (for Void Test)", paymentIntent);

                Console.WriteLine($"\n2. Syncing status for {paymentIntent.PaymentId} before attempting void...");
                paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                if (paymentIntent == null) { PrintAndReturnError("Payment Intent sync (pre-void) returned null or failed."); return null; }
                PrintPaymentDetails("2. After Sync (pre-void)", paymentIntent);

                if (paymentIntent.Status != "succeeded" && paymentIntent.Status != "failed" && paymentIntent.Status != "cancelled")
                {
                    Console.WriteLine($"\n3. Attempting to Cancel/Void Payment {paymentIntent.PaymentId} (Status: {paymentIntent.Status})...");
                    var cancelRequest = new PaymentCancelRequest { CancellationReason = "Test cancellation from SDK" };
                    paymentIntent = await paymentService.CancelPaymentAsync(paymentIntent.PaymentId!, cancelRequest);
                    if (paymentIntent == null) { PrintAndReturnError("Payment Cancel/Void call returned null or failed."); return null; }
                    PrintPaymentDetails("3. After Cancel/Void Attempt", paymentIntent);
                } else { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n3. Payment status is '{paymentIntent.Status}'. Void not attempted."); Console.ResetColor(); }
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in void payment flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in void payment flow"); }
            return paymentIntent;
        }

        static async Task<PaymentIntentResponse?> TestUpdatePaymentAsync(PaymentService paymentService)
        {
            Console.WriteLine($"Testing Update Payment Flow");
            PaymentIntentResponse? paymentIntent = null;
            try
            {
                Console.WriteLine("\n1. Creating Payment Intent for Update Test (confirm: false)...");
                var createRequest = new PaymentIntentRequest { Amount = 850, Currency = "USD", ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn", Confirm = false, CaptureMethod = "manual", PaymentMethod = "card", PaymentMethodType = "credit", Email = "customer-update-initial@example.com", Description = "Initial Description for Update Test", ReturnUrl = "https://example.com/sdk_update_return", PaymentMethodData = new PaymentMethodData { Card = new CardDetails { CardNumber = "4917610000000000", CardExpiryMonth = "03", CardExpiryYear = "2030", CardCvc = "737" }, Billing = new Address { AddressDetails = new AddressDetails { Line1 = "202 Update Ave", City="Modifyville", Country="US", Zip="11111"} } }, AuthenticationType = "no_three_ds", BrowserInfo = new BrowserInfo { ColorDepth = 24, JavaEnabled = true, JavaScriptEnabled = true, Language = "en-US", ScreenHeight = 720, ScreenWidth = 1280, TimeZone = -330, IpAddress = "208.127.127.196", AcceptHeader = "application/json", UserAgent = "Mozilla/5.0 SDK Update Test" } };
                paymentIntent = await paymentService.CreateAsync(createRequest);
                if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.PaymentId)) { PrintAndReturnError("PaymentId is missing after create for update test."); return null; }
                PrintPaymentDetails("1. After Create (for Update Test)", paymentIntent);

                Console.WriteLine($"\n2. Updating Payment Intent {paymentIntent.PaymentId}...");
                var updateRequest = new PaymentUpdateRequest { Amount = 900, Description = "Updated Description for Payment", Email = "customer-update-final@example.com", Metadata = new Dictionary<string, string> { { "updated_by", "sdk_sample" }, { "reason", "test_update_flow" } }, PaymentMethod = createRequest.PaymentMethod, PaymentMethodType = createRequest.PaymentMethodType, PaymentMethodData = createRequest.PaymentMethodData };
                paymentIntent = await paymentService.UpdatePaymentAsync(paymentIntent.PaymentId!, updateRequest);
                if (paymentIntent == null) { PrintAndReturnError("Payment Update call returned null or failed."); return null; }
                PrintPaymentDetails("2. After Update Attempt", paymentIntent);

                Console.WriteLine($"\n3. Syncing status for {paymentIntent.PaymentId} after update...");
                paymentIntent = await paymentService.SyncPaymentStatusAsync(paymentIntent.PaymentId!, clientSecret: paymentIntent.ClientSecret, forceSync: true);
                PrintPaymentDetails("3. After Sync (post-update)", paymentIntent);
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in update payment flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in update payment flow"); }
            return paymentIntent;
        }

        static async Task<RefundResponse?> TestRefundPaymentAsync(PaymentService paymentService, RefundService refundService, string? paymentIdToRefund)
        {
            Console.WriteLine($"Testing Refund Payment Flow for Payment ID: {paymentIdToRefund ?? "N/A"}");
            if (string.IsNullOrEmpty(paymentIdToRefund))
            {
                PrintAndReturnError("No valid Payment ID provided for refund test.");
                return null;
            }

            PaymentIntentResponse? paymentDetails = null;
            RefundResponse? createdRefund = null;
            try
            {
                Console.WriteLine($"\n1. Syncing status for payment {paymentIdToRefund} to ensure it's succeeded...");
                paymentDetails = await paymentService.SyncPaymentStatusAsync(paymentIdToRefund, clientSecret: null, forceSync: true); 
                if (paymentDetails == null) { PrintAndReturnError($"Failed to sync payment {paymentIdToRefund}."); return null; }
                PrintPaymentDetails("1. After Sync (pre-refund)", paymentDetails);

                if (paymentDetails.Status != "succeeded") { PrintAndReturnError($"Payment {paymentIdToRefund} is not 'succeeded' (Status: {paymentDetails.Status}). Cannot test refund."); return null; }

                Console.WriteLine($"\n2. Creating Refund for Payment {paymentDetails.PaymentId}...");
                var refundRequest = new RefundCreateRequest { PaymentId = paymentDetails.PaymentId!, Amount = paymentDetails.Amount / 2, Reason = "OTHER", Metadata = new Dictionary<string, string> { { "test_type", "sdk_refund_test" } } };
                createdRefund = await refundService.CreateRefundAsync(refundRequest);
                if (createdRefund == null || string.IsNullOrEmpty(createdRefund.RefundId)) { PrintAndReturnError("Refund creation returned null or failed to provide RefundId."); return null; }
                PrintRefundDetails("2. After Refund Create", createdRefund);

                Console.WriteLine($"\n3. Retrieving (rsync) Refund {createdRefund.RefundId} (initial check)...");
                RefundResponse? retrievedRefund = await refundService.RetrieveRefundAsync(createdRefund.RefundId!);
                PrintRefundDetails("3. After Initial Refund Retrieve", retrievedRefund);

                int refundSyncAttempts = 0;
                if (retrievedRefund != null && !string.IsNullOrEmpty(retrievedRefund.RefundId))
                {
                    while (retrievedRefund.Status == "pending" || retrievedRefund.Status == "requires_action")
                    {
                        if (refundSyncAttempts >= 3) { Console.WriteLine($"   Refund {retrievedRefund.RefundId} still '{retrievedRefund.Status}' after {refundSyncAttempts} sync attempts. Stopping poll."); break; }
                        refundSyncAttempts++;
                        Console.WriteLine($"\n   Syncing refund {retrievedRefund.RefundId} (attempt {refundSyncAttempts}), current status: {retrievedRefund.Status}...");
                        await Task.Delay(2000); 
                        retrievedRefund = await refundService.RetrieveRefundAsync(retrievedRefund.RefundId!);
                        if (retrievedRefund == null) { PrintAndReturnError("RetrieveRefundAsync returned null during polling."); break; }
                        PrintRefundDetails($"   After Refund Sync Attempt {refundSyncAttempts}", retrievedRefund);
                    }
                    if(retrievedRefund != null) { Console.WriteLine($"   Final polled status for refund {retrievedRefund.RefundId}: {retrievedRefund.Status}"); }
                }
                Console.WriteLine($"\n4. Syncing original payment {paymentDetails.PaymentId} again after refund process...");
                PaymentIntentResponse? paymentAfterRefund = await paymentService.SyncPaymentStatusAsync(paymentDetails.PaymentId!, clientSecret: null, forceSync: true); 
                PrintPaymentDetails("4. Original Payment Status After Refund Process", paymentAfterRefund);
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in refund payment flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in refund payment flow"); }
            return createdRefund; 
        }

        static async Task TestUpdateRefundAsync(RefundService refundService, string refundIdToUpdate)
        {
            Console.WriteLine($"Testing Update Refund Flow for Refund ID: {refundIdToUpdate}");
            try
            {
                Console.WriteLine($"\n1. Retrieving refund {refundIdToUpdate} before update...");
                RefundResponse? refundDetails = await refundService.RetrieveRefundAsync(refundIdToUpdate);
                if (refundDetails == null) { PrintAndReturnError($"Failed to retrieve refund {refundIdToUpdate} before update."); return; }
                PrintRefundDetails("1. Before Update Refund", refundDetails);

                if (refundDetails.Status == "pending" || refundDetails.Status == "requires_action")
                {
                    Console.WriteLine($"\n2. Updating refund {refundIdToUpdate}...");
                    var updateRequest = new RefundUpdateRequest { Reason = "Updated reason via SDK", Metadata = new Dictionary<string, string> { { "update_source", "sdk_sample_test" }, { "initial_reason", refundDetails.Reason ?? "N/A" } } };
                    RefundResponse? updatedRefund = await refundService.UpdateRefundAsync(refundIdToUpdate, updateRequest);
                    if (updatedRefund == null) { PrintAndReturnError("Refund Update call returned null or failed."); return; }
                    PrintRefundDetails("2. After Update Refund Attempt", updatedRefund);
                } else { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"\n2. Refund status is '{refundDetails.Status}'. Update not typically allowed/meaningful. Skipping update attempt."); Console.ResetColor(); }
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in update refund flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in update refund flow"); }
        }

        static async Task TestListRefundsAsync(RefundService refundService, string? paymentIdToList = null, int? limit = null)
        {
            if (!string.IsNullOrEmpty(paymentIdToList)) { Console.WriteLine($"Testing List Refunds Flow for Payment ID: {paymentIdToList}"); }
            else { Console.WriteLine($"Testing List Refunds Flow (general list with limit {limit?.ToString() ?? "default"})"); }
            
            try
            {
                var listRequest = new RefundListRequest();
                if (!string.IsNullOrEmpty(paymentIdToList)) { listRequest.PaymentId = paymentIdToList; }
                if (limit.HasValue) { listRequest.Limit = limit; }

                Console.WriteLine("\n1. Listing refunds...");
                RefundListResponse? refunds = await refundService.ListRefundsAsync(listRequest);
                
                if (refunds == null) { PrintAndReturnError("List Refunds call returned null."); return; }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Found {refunds.Data?.Count ?? 0} refund(s). (API reported: Count={refunds.Count}, TotalCount={refunds.TotalCount})");
                Console.ResetColor();
                int count = 0;
                if (refunds.Data != null)
                {
                    foreach (var refund in refunds.Data)
                    {
                        PrintRefundDetails($"Listed Refund {++count}", refund);
                    }
                }
                if (refunds.Data == null || refunds.Data.Count == 0) { Console.WriteLine("No refunds found matching criteria in the current list page.");}
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in list refunds flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in list refunds flow"); }
        }

        static async Task TestListCustomerPaymentMethodsAsync(CustomerService customerService, string customerId)
        {
            Console.WriteLine($"Testing List Customer Payment Methods for Customer ID: {customerId}");
            if (string.IsNullOrEmpty(customerId))
            {
                PrintAndReturnError("Customer ID is required to list payment methods.");
                return;
            }
            try
            {
                Console.WriteLine("\n1. Listing customer payment methods...");
                CustomerPaymentMethodListResponse? response = await customerService.ListPaymentMethodsAsync(customerId);

                if (response == null) { PrintAndReturnError("List Customer Payment Methods call returned null."); return; }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Customer ID: {response.CustomerId}");
                Console.WriteLine($"Payment Method Count: {response.PaymentMethodCount}");
                Console.ResetColor();

                if (response.Data != null && response.Data.Any())
                {
                    int count = 0;
                    foreach (var pm in response.Data)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine($"  Payment Method {++count}:");
                        Console.WriteLine($"    Token: {pm.PaymentToken}");
                        Console.WriteLine($"    Method: {pm.PaymentMethod}, Type: {pm.PaymentMethodType}, Issuer: {pm.PaymentMethodIssuer}");
                        Console.WriteLine($"    Recurring: {pm.RecurringEnabled}, Installment: {pm.InstallmentPaymentEnabled}");
                        Console.WriteLine($"    Created: {pm.Created}");
                        if (pm.Card != null)
                        {
                            Console.WriteLine($"    Card: Last4={pm.Card.Last4Digits}, Expires={pm.Card.ExpiryMonth}/{pm.Card.ExpiryYear}");
                        }
                        if (pm.Metadata != null && pm.Metadata.Any())
                        {
                            Console.WriteLine("    Metadata:");
                            foreach(var meta in pm.Metadata) { Console.WriteLine($"      {meta.Key}: {meta.Value}"); }
                        }
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine("No saved payment methods found for this customer.");
                }
            }
            catch (HyperswitchApiException apiEx) { PrintApiError(apiEx, "in list customer payment methods flow"); }
            catch (Exception ex) { PrintGenericError(ex, "in list customer payment methods flow"); }
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
        
        static void PrintRefundDetails(string stage, RefundResponse? refund)
        {
            if (refund == null) { PrintAndReturnError($"{stage}: Refund details are null."); return; }
            Console.ForegroundColor = ConsoleColor.DarkYellow; 
            Console.WriteLine($"{stage}:");
            Console.WriteLine($"  Refund ID: {refund.RefundId}, Payment ID: {refund.PaymentId}");
            Console.WriteLine($"  Amount: {refund.Amount} {refund.Currency}, Status: {refund.Status}");
            Console.WriteLine($"  Reason: {refund.Reason}, CreatedAt: {refund.CreatedAt}");
            if (!string.IsNullOrEmpty(refund.ErrorCode)) { Console.WriteLine($"  Error: [{refund.ErrorCode}] {refund.ErrorMessage}");}
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
