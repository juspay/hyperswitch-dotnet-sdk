using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HyperSwitch.Net.Customers.Exceptions;
using HyperSwitch.Net.Customers.Models; // Assuming error response might be structured

namespace HyperSwitch.Net.Customers
{
    public class HyperSwitchHttpClient : IHyperSwitchHttpClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        // _apiKey is not directly used in SendAsync after constructor setup, 
        // but kept for completeness or if other methods might need it.
        // private readonly string _apiKey; 

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            // Add any other preferred default options here
        };

        public HyperSwitchHttpClient(string baseUrl, string apiKey, HttpClient? httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));
            if (string.IsNullOrWhiteSpace(apiKey)) // apiKey is used for setup
                throw new ArgumentNullException(nameof(apiKey));

            // _apiKey = apiKey; // Store if needed for other direct uses
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("api-key", apiKey); // Use apiKey directly
        }

        public async Task<(TResponse? Response, HyperSwitchApiException? Error)> SendAsync<TRequest, TResponse>(
            HttpMethod method, 
            string relativeUrl, 
            TRequest? requestBody = default, 
            CancellationToken cancellationToken = default) 
            where TRequest : class 
            where TResponse : class
        {
            HttpContent? content = null;
            if (requestBody != null)
            {
                var jsonRequestBody = JsonSerializer.Serialize(requestBody, JsonSerializerOptions);
                content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            }

            using var requestMessage = new HttpRequestMessage(method, relativeUrl)
            {
                Content = content
            };
            
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            }
            catch (TaskCanceledException ex) // Handles HttpClient timeout
            {
                // It's good practice to differentiate between cancellation by token and timeout
                if (cancellationToken.IsCancellationRequested)
                {
                    return (null, new HyperSwitchApiException(System.Net.HttpStatusCode.RequestTimeout, "RequestCancelled", "The request was cancelled by the caller.", ex.Message, ex));
                }
                return (null, new HyperSwitchApiException(System.Net.HttpStatusCode.RequestTimeout, "RequestTimeout", "The request timed out.", ex.Message, ex));
            }
            catch (HttpRequestException ex) // Handles network errors
            {
                return (null, new HyperSwitchApiException(0, "NetworkError", "A network error occurred.", ex.Message, ex));
            }

            if (response.IsSuccessStatusCode)
            {
                if (response.Content.Headers.ContentLength == 0)
                {
                    return (null, null); // Or handle as appropriate (e.g., TResponse could be a specific NoResult type)
                }
                try
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var deserializedResponse = await JsonSerializer.DeserializeAsync<TResponse>(responseStream, JsonSerializerOptions, cancellationToken);
                    return (deserializedResponse, null);
                }
                catch (JsonException ex)
                {
                    return (null, new HyperSwitchApiException(response.StatusCode, "DeserializationError", "Failed to deserialize the response.", ex.Message, ex));
                }
            }
            else
            {
                string errorContent = string.Empty;
                try
                {
                     errorContent = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
                {
                    // Handle cases where reading error content itself is cancelled or times out
                    return (null, new HyperSwitchApiException(response.StatusCode, "ErrorReadCancelled", "Failed to read error content due to cancellation/timeout.", ex.Message, ex));
                }
                catch (Exception ex)
                {
                    // Catch other potential errors during error content reading
                    return (null, new HyperSwitchApiException(response.StatusCode, "ErrorReadFailed", "Failed to read error content.", ex.Message, ex));
                }
                
                string? apiErrorCode = null;
                string? apiErrorMessage = null;
                string reason = errorContent;

                try
                {
                    var parsedError = JsonDocument.Parse(errorContent);
                    if (parsedError.RootElement.TryGetProperty("code", out var codeElement))
                    {
                        apiErrorCode = codeElement.GetString();
                    }
                    if (parsedError.RootElement.TryGetProperty("message", out var messageElement))
                    {
                        apiErrorMessage = messageElement.GetString();
                    }
                    if (parsedError.RootElement.TryGetProperty("reason", out var reasonElement) && reasonElement.GetString() is string r)
                    {
                        reason = r;
                    } 
                    else if (apiErrorMessage != null) 
                    {
                        reason = apiErrorMessage;
                    }
                }
                catch (JsonException) // Ignored if errorContent is not JSON
                {
                     reason = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}. Raw response: {errorContent}";
                }
                return (null, new HyperSwitchApiException(response.StatusCode, apiErrorCode, apiErrorMessage, reason));
            }
        }
        
        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
} 