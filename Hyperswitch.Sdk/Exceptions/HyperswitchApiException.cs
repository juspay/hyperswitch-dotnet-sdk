using System;
using System.Text.Json; // Added for JsonException
using Hyperswitch.Sdk.Models; // Will be created later

namespace Hyperswitch.Sdk.Exceptions
{
    public class HyperswitchApiException : Exception
    {
        public int StatusCode { get; }
        public string? ResponseContent { get; }
        public ErrorResponse? ErrorDetails { get; }

        public HyperswitchApiException(string message, int statusCode, string? responseContent = null, ErrorResponse? errorDetails = null, Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
            ErrorDetails = errorDetails;
        }

        public HyperswitchApiException(string message, int statusCode, string? responseContent, JsonException innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
            ErrorDetails = null; // Deserialization of ErrorResponse failed
        }
    }
}
