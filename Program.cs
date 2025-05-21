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
            Console.WriteLine("Hyperswitch SDK Sample - cURL Aligned Request");

            string apiKey = "snd_EOQGADPyoizCvcr733PMNPX1GfO5UyalBwqMRGki8RqMyEMc7BtD2sSSFEUMCnJQ"; 

            var client = new HyperswitchClient(apiKey);
            var paymentService = new PaymentService(client);

            try
            {
                Console.WriteLine("\nAttempting to create a payment intent (cURL aligned)...");
                var paymentIntentRequest = new PaymentIntentRequest
                {
                    Amount = 700,
                    Currency = "USD",
                    ProfileId = "pro_QqM6TOJtfvsbp6VSvhIn",
                    Confirm = true,
                    PaymentLink = false,
                    CaptureMethod = "manual",
                    CaptureOn = "2029-09-10T10:11:12Z",
                    AmountToCapture = 700,
                    Name = "John Doe",
                    Email = "dg@example.com",
                    Phone = "999999999",
                    PhoneCountryCode = "+1",
                    Description = "Its my first payment request",
                    AuthenticationType = "no_three_ds",
                    ReturnUrl = "https://google.com",
                    PaymentMethodData = new PaymentMethodData
                    {
                        // Type = "card", // Not explicitly in cURL for this part, but SDK model has it. Hyperswitch might infer.
                        Card = new CardDetails
                        {
                            CardNumber = "4917610000000000",
                            CardExpiryMonth = "03",
                            CardExpiryYear = "2030",
                            CardCvc = "737"
                        },
                        Billing = new Address // Billing specific to payment_method_data
                        {
                            AddressDetails = new AddressDetails
                            {
                                Line1 = "1467",
                                Line2 = "CA",
                                City = "San Fransico", // Typo from cURL: "San Fransico"
                                State = "California",
                                Zip = "94122",
                                Country = "US"
                            }
                            // No phone details under payment_method_data.billing in cURL
                        }
                    },
                    PaymentMethod = "card",
                    PaymentMethodType = "credit",
                    BrowserInfo = new BrowserInfo
                    {
                        ColorDepth = 24,
                        JavaEnabled = true,
                        JavaScriptEnabled = true,
                        Language = "en-GB",
                        ScreenHeight = 720,
                        ScreenWidth = 1280,
                        TimeZone = -330,
                        IpAddress = "208.127.127.193",
                        AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.0.0"
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "udf1", "value1" },
                        { "new_customer", "true" },
                        { "login_date", "2019-09-10T10:11:12Z" }
                    },
                    OrderDetails = new List<OrderDetailItem>
                    {
                        new OrderDetailItem { ProductName = "Tea", Quantity = 1, Amount = 110, ProductImgLink = "https://thumbs.dreamstime.com/b/indian-tea-spices-masala-chai-33827904.jpg" },
                        new OrderDetailItem { ProductName = "Tea", Quantity = 1, Amount = 110, ProductImgLink = "https://thumbs.dreamstime.com/b/indian-tea-spices-masala-chai-33827904.jpg" },
                        new OrderDetailItem { ProductName = "Tea", Quantity = 1, Amount = 110, ProductImgLink = "https://thumbs.dreamstime.com/b/indian-tea-spices-masala-chai-33827904.jpg" }
                    }
                    // Top-level shipping and billing are not in the cURL, so they are omitted here.
                };

                PaymentIntentResponse? createdPayment = await paymentService.CreateAsync(paymentIntentRequest);

                if (createdPayment != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Payment Intent created successfully!");
                    Console.WriteLine($"  Payment ID: {createdPayment.PaymentId}");
                    Console.WriteLine($"  Status: {createdPayment.Status}");
                    Console.WriteLine($"  Amount: {createdPayment.Amount} {createdPayment.Currency}");
                    Console.WriteLine($"  Client Secret: {createdPayment.ClientSecret}");
                    if (!string.IsNullOrEmpty(createdPayment.NextAction?.RedirectToUrl)) // Check if the string URL is present
                    {
                        Console.WriteLine($"  Next Action URL: {createdPayment.NextAction.RedirectToUrl}"); // Use the string directly
                    }
                    Console.ResetColor();

                    if (!string.IsNullOrEmpty(createdPayment.PaymentId))
                    {
                        Console.WriteLine($"\nAttempting to sync payment status for {createdPayment.PaymentId} (forceSync=true)...");
                        PaymentIntentResponse? syncedPayment = await paymentService.SyncPaymentStatusAsync(createdPayment.PaymentId, createdPayment.ClientSecret, forceSync: true);
                        
                        if (syncedPayment != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Payment Intent synced successfully!");
                            Console.WriteLine($"  Payment ID: {syncedPayment.PaymentId}");
                            Console.WriteLine($"  Status (after sync): {syncedPayment.Status}");
                            Console.WriteLine($"  Amount: {syncedPayment.Amount} {syncedPayment.Currency}");
                             Console.WriteLine($"  Amount Capturable: {syncedPayment.AmountCapturable}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Could not sync the payment intent, or the response was null.");
                            Console.ResetColor();
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Payment intent creation returned null.");
                    Console.ResetColor();
                }
            }
            catch (HyperswitchApiException apiEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Hyperswitch API Error:");
                Console.WriteLine($"  Status Code: {apiEx.StatusCode}");
                Console.WriteLine($"  Message: {apiEx.Message}");
                if (apiEx.ErrorDetails?.Error != null)
                {
                    Console.WriteLine($"  Error Code: {apiEx.ErrorDetails.Error.Code}");
                    Console.WriteLine($"  Error Message: {apiEx.ErrorDetails.Error.Message}");
                }
                Console.WriteLine($"  Raw Response: {apiEx.ResponseContent}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
