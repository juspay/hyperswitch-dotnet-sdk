using System;
using System.Net;

namespace HyperSwitch.Net.Customers.Exceptions
{
    public class HyperSwitchApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string? ApiErrorCode { get; }
        public string? ApiErrorMessage { get; }

        public HyperSwitchApiException(HttpStatusCode statusCode, string? apiErrorCode, string? apiErrorMessage, string? message, Exception? innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ApiErrorCode = apiErrorCode;
            ApiErrorMessage = apiErrorMessage;
        }

        public HyperSwitchApiException(HttpStatusCode statusCode, string? apiErrorCode, string? apiErrorMessage, string? message)
            : this(statusCode, apiErrorCode, apiErrorMessage, message, null)
        {
        }

        public HyperSwitchApiException(HttpStatusCode statusCode, string? message)
            : this(statusCode, null, null, message, null)
        {
        }

        public HyperSwitchApiException(string message, Exception innerException)
            : this(0, null, null, message, innerException) // Default status code if not provided
        {
        }

        public HyperSwitchApiException(string message)
            : this(0, null, null, message, null) // Default status code if not provided
        {
        }
    }
} 