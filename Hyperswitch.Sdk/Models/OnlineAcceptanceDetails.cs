using System.Text.Json.Serialization;

namespace Hyperswitch.Sdk.Models
{
    /// <summary>
    /// Represents the details of an online customer acceptance.
    /// </summary>
    public class OnlineAcceptanceDetails
    {
        /// <summary>
        /// Gets or sets the user agent of the customer's browser.
        /// </summary>
        [JsonPropertyName("user_agent")]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the customer.
        /// (Optional, but often included in online acceptance)
        /// </summary>
        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; } 
    }
}
