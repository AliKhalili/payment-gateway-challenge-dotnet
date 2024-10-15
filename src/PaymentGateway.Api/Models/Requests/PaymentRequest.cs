using System.Text.Json.Serialization;

using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models
{
    public class PaymentRequest
    {
        [JsonPropertyName("card_number")]
        public required string CardNumber { get; set; }


        [JsonPropertyName("expiry_month")]
        public required int ExpiryMonth { get; set; }


        [JsonPropertyName("expiry_year")]
        public required int ExpiryYear { get; set; }


        [JsonPropertyName("currency")]
        public required PaymentCurrency Currency { get; set; }


        [JsonPropertyName("amount")]
        public required long Amount { get; set; }


        [JsonPropertyName("cvv")]
        public required string Cvv { get; set; }
    }
}