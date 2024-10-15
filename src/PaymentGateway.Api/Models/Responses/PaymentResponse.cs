using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses
{
    public class PaymentResponse
    {
        public string Id { get; init; }
        public string Status { get; init; }
        public string LastFourCardDigiti { get; init; }
        public int ExpiryMonth { get; init; }
        public int ExpiryYear { get; init; }
        public PaymentCurrency Currency { get; init; }
        public long Amount { get; init; }
    }
}