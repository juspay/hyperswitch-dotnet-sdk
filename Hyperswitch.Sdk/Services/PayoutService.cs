using Hyperswitch.Sdk.Models;

namespace Hyperswitch.Sdk.Services
{
    /// <summary>
    /// Provides methods for interacting with the Hyperswitch Payout Intent API.
    /// </summary>
    public class PayoutService
    {
        private readonly HyperswitchClient _client;
        private const string PayoutsUrl = "/payouts"; // Base path for payouts API

        /// <summary>
        /// Initializes a new instance of the <see cref="PayoutService"/> class.
        /// </summary>
        /// <param name="client">The <see cref="HyperswitchClient"/> to use for API calls.</param>
        public PayoutService(HyperswitchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a new payout intent.
        /// </summary>
        /// <param name="request">The payout intent creation request.</param>
        /// <returns>The created payout intent.</returns>
        public async Task<PayoutIntentResponse?> CreateAsync(PayoutIntentRequest request)
        {
            if (request == null)
                throw new System.ArgumentNullException(nameof(request));

            // Ensure ProfileId is set if not provided in request and a default is available
            if (string.IsNullOrWhiteSpace(request.ProfileId) && !string.IsNullOrWhiteSpace(_client.DefaultProfileId))
            {
                request.ProfileId = _client.DefaultProfileId;
                System.Console.WriteLine($"[PayoutService DEBUG] Using default ProfileId: {request.ProfileId} for Payout Intent creation.");
            }
            string requestUrl = $"{PayoutsUrl}/create";
            return await _client.PostAsync<PayoutIntentRequest, PayoutIntentResponse>(requestUrl, request);
        }
    }
}
