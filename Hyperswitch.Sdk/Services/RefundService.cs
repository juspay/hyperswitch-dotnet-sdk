using System.Threading.Tasks;
using Hyperswitch.Sdk.Models;

namespace Hyperswitch.Sdk.Services
{
    public class RefundService
    {
        private readonly HyperswitchClient _client;
        private const string RefundsUrl = "/refunds"; // Base path for refund operations (create, retrieve by ID, update by ID)
        private const string RefundListUrl = "/refunds/list"; // Path for listing refunds

        public RefundService(HyperswitchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a new refund for a payment.
        /// </summary>
        /// <param name="request">The refund creation request.</param>
        /// <returns>The created refund object.</returns>
        public async Task<RefundResponse?> CreateRefundAsync(RefundCreateRequest request)
        {
            if (request == null)
                throw new System.ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.PaymentId))
                throw new System.ArgumentException("PaymentId is required in the RefundCreateRequest.", nameof(request.PaymentId));

            return await _client.PostAsync<RefundCreateRequest, RefundResponse>(RefundsUrl, request);
        }

        /// <summary>
        /// Retrieves an existing refund.
        /// </summary>
        /// <param name="refundId">The ID of the refund to retrieve.</param>
        /// <returns>The retrieved refund object.</returns>
        public async Task<RefundResponse?> RetrieveRefundAsync(string refundId)
        {
            if (string.IsNullOrWhiteSpace(refundId))
                throw new System.ArgumentNullException(nameof(refundId));

            string requestUrl = $"{RefundsUrl}/{refundId}"; 
            
            return await _client.GetAsync<RefundResponse>(requestUrl);
        }

        /// <summary>
        /// Updates an existing refund.
        /// </summary>
        /// <param name="refundId">The ID of the refund to update.</param>
        /// <param name="request">The details to update on the refund.</param>
        /// <returns>The updated refund object.</returns>
        public async Task<RefundResponse?> UpdateRefundAsync(string refundId, RefundUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(refundId))
                throw new System.ArgumentNullException(nameof(refundId));
            if (request == null)
                throw new System.ArgumentNullException(nameof(request));

            string requestUrl = $"{RefundsUrl}/{refundId}"; 
            
            return await _client.PostAsync<RefundUpdateRequest, RefundResponse>(requestUrl, request);
        }

        /// <summary>
        /// Lists refunds, optionally filtered by request parameters sent in the body.
        /// </summary>
        /// <param name="request">(Optional) Parameters to filter and paginate the list of refunds.</param>
        /// <returns>A list of refund objects.</returns>
        public async Task<RefundListResponse?> ListRefundsAsync(RefundListRequest? request = null)
        {
            return await _client.PostAsync<RefundListRequest?, RefundListResponse>(RefundListUrl, request);
        }
    }
}
