using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public ErrorDetails? Error { get; set; }
    }

    public class ErrorDetails
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        // Depending on Hyperswitch API, there might be other fields like 'param', 'type', etc.
        // These can be added later if needed.
    }
}
