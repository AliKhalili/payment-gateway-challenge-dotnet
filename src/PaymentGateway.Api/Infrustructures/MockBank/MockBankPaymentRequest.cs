using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Infrastructures.MockBank
{
    public class MockBankPaymentRequest
    {
        [JsonPropertyName("card_number")]
        public string CardNumber { get; init; }

        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("amount")]
        public long Amount { get; init; }

        [JsonPropertyName("cvv")]
        public string Cvv { get; init; }
    }
}