using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HyperSwitch.Net.Customers.Exceptions; // Assuming HyperSwitchApiException is in this namespace

namespace HyperSwitch.Net.Customers
{
    public interface IHyperSwitchHttpClient
    {
        Task<(TResponse? Response, HyperSwitchApiException? Error)> SendAsync<TRequest, TResponse>(
            HttpMethod method,
            string relativeUrl,
            TRequest? requestBody = default,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : class;

        // If there are other public methods in HyperSwitchHttpClient that CustomerService uses,
        // they should be added here as well. For now, assuming SendAsync is the only one.
    }
} 