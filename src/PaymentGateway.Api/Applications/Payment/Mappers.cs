using PaymentGateway.Api.Infrastructures;
using PaymentGateway.Api.Infrastructures.MockBank;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Applications.Payment
{
    public static class Mappers
    {
        public static MockBankPaymentRequest ToMockBankRequest(this PaymentRequest paymentRequest)
        {
            return new MockBankPaymentRequest
            {
                Amount = paymentRequest.Amount,
                CardNumber = paymentRequest.CardNumber,
                Currency = paymentRequest.Currency.ToString(),
                Cvv = paymentRequest.Cvv,
                ExpiryDate = $"{paymentRequest.ExpiryMonth:00}/{paymentRequest.ExpiryYear}"
            };
        }

        public static GetPaymentResponse ToGetPaymentResponse(this StorePaymentModel model)
        {
            return new GetPaymentResponse()
            {
                Id = model.PaymentId,
                Status = model.Status,
                CardNumberLastFour = model.CardNumberLastFour,
                ExpiryMonth = model.ExpiryMonth,
                ExpiryYear = model.ExpiryYear,
                Currency = model.Currency.ToString(),
                Amount = model.Amount,
            };
        }

        public static PostPaymentResponse ToPostPaymentResponse(this StorePaymentModel model)
        {
            return new PostPaymentResponse()
            {
                Id = model.PaymentId,
                Status = model.Status,
                CardNumberLastFour = model.CardNumberLastFour,
                ExpiryMonth = model.ExpiryMonth,
                ExpiryYear = model.ExpiryYear,
                Currency = model.Currency.ToString(),
                Amount = model.Amount,
            };
        }
    }
}