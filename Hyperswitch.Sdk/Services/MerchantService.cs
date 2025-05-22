using System.Threading.Tasks;
using Hyperswitch.Sdk.Models;
using System.Collections.Generic; // Required for List<string> in queryParams

namespace Hyperswitch.Sdk.Services
{
    public class MerchantService
    {
        private readonly HyperswitchClient _client;
        // Corrected endpoint based on user-provided cURL.
        private const string AccountPaymentMethodsUrl = "/account/payment_methods"; 

        public MerchantService(HyperswitchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Lists the payment method types available and configured for the merchant account.
        /// </summary>
        /// <param name="request">(Optional) Parameters to filter the list, e.g., by country, currency, amount. 
        /// These are sent as query parameters if the API supports them.</param>
        /// <returns>A list of available payment method type information.</returns>
        public async Task<MerchantPMLResponse?> ListAvailablePaymentMethodsAsync(MerchantPMLRequest? request = null)
        {
            string requestUrl = AccountPaymentMethodsUrl;
            
            if (request != null)
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(request.Country))
                    queryParams.Add($"country={System.Uri.EscapeDataString(request.Country)}");
                if (!string.IsNullOrWhiteSpace(request.Currency))
                    queryParams.Add($"currency={System.Uri.EscapeDataString(request.Currency)}");
                if (request.Amount.HasValue)
                    queryParams.Add($"amount={request.Amount.Value}");
                if (!string.IsNullOrWhiteSpace(request.ProfileId))
                    queryParams.Add($"profile_id={System.Uri.EscapeDataString(request.ProfileId)}");
                
                if (queryParams.Count > 0)
                {
                    requestUrl += "?" + string.Join("&", queryParams);
                }
            }
            // Assumes the response is a direct list of MerchantPaymentMethodTypeInfo objects.
            // If the API returns a wrapper object (e.g. with pagination), MerchantPMLResponse will need to be a class
            // and this call would be _client.GetAsync<ActualWrapperType>(requestUrl);
            return await _client.GetAsync<MerchantPMLResponse>(requestUrl);
        }
    }
}
