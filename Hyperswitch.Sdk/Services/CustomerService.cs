using System.Threading.Tasks;
using Hyperswitch.Sdk.Models;

namespace Hyperswitch.Sdk.Services
{
    public class CustomerService
    {
        private readonly HyperswitchClient _client;
        private const string CustomersUrl = "/customers"; 

        public CustomerService(HyperswitchClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Lists the saved payment methods for a specific customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer whose payment methods are to be listed.</param>
        /// <returns>A response object containing a list of the customer's saved payment methods.</returns>
        public async Task<CustomerPaymentMethodListResponse?> ListPaymentMethodsAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new System.ArgumentNullException(nameof(customerId));

            string requestUrl = $"{CustomersUrl}/{customerId}/payment_methods";
            
            return await _client.GetAsync<CustomerPaymentMethodListResponse>(requestUrl);
        }
    }
}
