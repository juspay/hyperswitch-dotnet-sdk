using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using HyperSwitch.Net.Customers.Models;
using HyperSwitch.Net.Customers.Exceptions;

namespace HyperSwitch.Net.Customers
{
    public class CustomerService
    {
        private readonly IHyperSwitchHttpClient _httpClient;
        private const string CustomersBasePath = "/customers";

        public CustomerService(IHyperSwitchHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerResponse?> CreateCustomerAsync(CustomerRequest customerRequest, CancellationToken cancellationToken = default)
        {
            if (customerRequest == null) throw new System.ArgumentNullException(nameof(customerRequest));

            var (response, error) = await _httpClient.SendAsync<CustomerRequest, CustomerResponse>(
                HttpMethod.Post,
                CustomersBasePath,
                customerRequest,
                cancellationToken
            );
            if (error != null) throw error;
            return response;
        }

        public async Task<CustomerResponse?> RetrieveCustomerAsync(string customerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId)) throw new System.ArgumentNullException(nameof(customerId));

            var (response, error) = await _httpClient.SendAsync<object, CustomerResponse>(
                HttpMethod.Get,
                $"{CustomersBasePath}/{customerId}",
                null, // No request body for GET
                cancellationToken
            );
            if (error != null) throw error;
            return response;
        }

        public async Task<CustomerResponse?> UpdateCustomerAsync(string customerId, CustomerUpdateRequest customerUpdateRequest, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId)) throw new System.ArgumentNullException(nameof(customerId));
            if (customerUpdateRequest == null) throw new System.ArgumentNullException(nameof(customerUpdateRequest));

            var (response, error) = await _httpClient.SendAsync<CustomerUpdateRequest, CustomerResponse>(
                HttpMethod.Post, // As per OpenAPI spec, update is POST to /customers/{customerId}
                $"{CustomersBasePath}/{customerId}",
                customerUpdateRequest,
                cancellationToken
            );
            if (error != null) throw error;
            return response;
        }

        public async Task<CustomerDeleteResponse?> DeleteCustomerAsync(string customerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId)) throw new System.ArgumentNullException(nameof(customerId));

            var (response, error) = await _httpClient.SendAsync<object, CustomerDeleteResponse>(
                HttpMethod.Delete,
                $"{CustomersBasePath}/{customerId}",
                null, // No request body for DELETE
                cancellationToken
            );
            if (error != null) throw error;
            return response;
        }

        public async Task<List<CustomerResponse>?> ListCustomersAsync(CancellationToken cancellationToken = default /* Potential query parameters like limit, offset, email etc. */)
        {
            var (response, error) = await _httpClient.SendAsync<object, List<CustomerResponse>>(
                HttpMethod.Get,
                $"{CustomersBasePath}/list",
                null, // No request body for GET
                cancellationToken
            );
            if (error != null) throw error;
            return response;
        }
    }
} 