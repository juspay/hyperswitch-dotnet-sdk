using System.Threading.Tasks;
using Hyperswitch.Sdk.Models;

namespace Hyperswitch.Sdk.Services
{
    public class PaymentService
    {
        private readonly HyperswitchClient _client;
        private const string PaymentsUrl = "/payments"; // Base path for payments API

        public PaymentService(HyperswitchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a new payment intent.
        /// </summary>
        /// <param name="request">The payment intent creation request.</param>
        /// <returns>The created payment intent.</returns>
        public async Task<PaymentIntentResponse?> CreateAsync(PaymentIntentRequest request)
        {
            if (request == null)
                throw new System.ArgumentNullException(nameof(request));

            return await _client.PostAsync<PaymentIntentRequest, PaymentIntentResponse>(PaymentsUrl, request);
        }

        /// <summary>
        /// Retrieves an existing payment intent.
        /// </summary>
        /// <param name="paymentId">The ID of the payment intent to retrieve.</param>
        /// <param name="clientSecret">(Optional) The client secret of the payment intent. 
        /// Recommended for fetching payment intent status from client-side after redirection.</param>
        /// <returns>The retrieved payment intent.</returns>
        public async Task<PaymentIntentResponse?> RetrieveAsync(string paymentId, string? clientSecret = null)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                throw new System.ArgumentNullException(nameof(paymentId));

            string requestUrl = $"{PaymentsUrl}/{paymentId}";
            // client_secret is typically not sent as a query param for server-side GET by ID
            // if (!string.IsNullOrWhiteSpace(clientSecret))
            // {
            //     requestUrl += $"?client_secret={clientSecret}";
            // }
            
            return await _client.GetAsync<PaymentIntentResponse>(requestUrl);
        }

        // Other payment-related methods (Update, Confirm, Capture, List, etc.) will be added later.

        /// <summary>
        /// Synchronizes and retrieves the latest status of an existing payment intent.
        /// </summary>
        /// <param name="paymentId">The ID of the payment intent to sync.</param>
        /// <param name="clientSecret">(Optional) The client secret of the payment intent.</param>
        /// <param name="forceSync">(Optional) If true, forces a sync with the payment processor. Defaults to false.</param>
        /// <returns>The latest payment intent details.</returns>
        public async Task<PaymentIntentResponse?> SyncPaymentStatusAsync(string paymentId, string? clientSecret = null, bool forceSync = false)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                throw new System.ArgumentNullException(nameof(paymentId));

            string requestUrl = $"{PaymentsUrl}/{paymentId}";
            
            var queryParameters = new System.Collections.Generic.List<string>();

            // client_secret is typically not sent as a query param for server-side GET by ID
            // if (!string.IsNullOrWhiteSpace(clientSecret))
            // {
            //     queryParameters.Add($"client_secret={System.Uri.EscapeDataString(clientSecret)}");
            // }

            if (forceSync)
            {
                queryParameters.Add($"force_sync={forceSync.ToString().ToLowerInvariant()}"); 
            }

            if (queryParameters.Count > 0)
            {
                requestUrl += "?" + string.Join("&", queryParameters);
            }
                    
            return await _client.GetAsync<PaymentIntentResponse>(requestUrl);
        }
    }
}
