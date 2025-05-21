using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Hyperswitch.Sdk.Exceptions; // Will be created later
using Hyperswitch.Sdk.Models;     // Will be created later

namespace Hyperswitch.Sdk
{
    public class HyperswitchClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private static readonly string DefaultBaseUrl = "https://sandbox.hyperswitch.io";

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public HyperswitchClient(string apiKey, string? baseUrl = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");

            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl ?? DefaultBaseUrl)
            };
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey); // Old auth
            _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey); // New auth based on cURL
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        internal async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload, JsonSerializerOptions);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            return await HandleResponse<TResponse>(response);
        }

        internal async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<TResponse>(response);
        }

        private async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    // Handle cases where the response is successful but has no content,
                    // and TResponse might be a value type or expect no content.
                    // If TResponse is a reference type, this might return null.
                    // If TResponse is a value type (e.g. bool for a 204 No Content),
                    // this might cause issues if not handled carefully by the caller or if default(TResponse) is not appropriate.
                    // For now, we assume TResponse can be null or default.
                    return default(TResponse)!;
                }
                try
                {
                    return JsonSerializer.Deserialize<TResponse>(responseContent, JsonSerializerOptions)!;
                }
                catch (JsonException ex)
                {
                    throw new HyperswitchApiException($"Failed to deserialize response: {ex.Message}. Response content: {responseContent}", (int)response.StatusCode, responseContent, ex);
                }
            }

            ErrorResponse? errorResponse = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, JsonSerializerOptions);
                }
            }
            catch (JsonException)
            {
                // Ignore if error response itself is not valid JSON
            }

            throw new HyperswitchApiException(
                errorResponse?.Error?.Message ?? $"API request failed with status code {response.StatusCode}.",
                (int)response.StatusCode,
                responseContent,
                errorResponse);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
