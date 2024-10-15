using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Infrastructures.MockBank
{
    public class MockBankPaymentResponse
    {
        [JsonPropertyName("authorized")]
        public bool Authorized { get; init; }

        [JsonPropertyName("authorization_code")]
        public string AuthorizedCode { get; init; }
    }
}