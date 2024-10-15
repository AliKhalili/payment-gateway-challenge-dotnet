using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Infrastructures
{
    public class StorePaymentModel
    {
        public string PaymentId { get; }
        public PaymentStatus Status { get; }
        public int CardNumberLastFour { get; }
        public int ExpiryMonth { get; }
        public int ExpiryYear { get; }
        public PaymentCurrency Currency { get; }
        public long Amount { get; }

        public StorePaymentModel(string paymentId, PaymentStatus paymentStatus, PaymentRequest paymentRequest)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);
            ArgumentNullException.ThrowIfNull(paymentStatus);
            ArgumentNullException.ThrowIfNull(paymentRequest);

            PaymentId = paymentId;
            Status = paymentStatus;
            ExpiryMonth = paymentRequest.ExpiryMonth;
            ExpiryYear = paymentRequest.ExpiryYear;
            Currency = paymentRequest.Currency;
            Amount = paymentRequest.Amount;
            CardNumberLastFour = int.Parse(paymentRequest.CardNumber.Substring(paymentRequest.CardNumber.Length - 4));
        }
    }
}