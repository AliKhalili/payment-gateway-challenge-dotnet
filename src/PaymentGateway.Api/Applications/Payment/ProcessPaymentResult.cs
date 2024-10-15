
using FluentValidation.Results;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Applications.Payment
{
    public class ProcessPaymentDone(string paymentId, PaymentStatus paymentStatus, PostPaymentResponse paymentResponse)
    {
        public string PaymentId { get; init; } = paymentId;
        public PaymentStatus PaymentStatus { get; init; } = paymentStatus;
        public PostPaymentResponse PostPaymentResponse { get; init; } = paymentResponse;
    }

    public class ProcessPaymentRejected(string paymentId, ValidationResult validationResult)
    {
        public string PaymentId { get; init; } = paymentId;
        public readonly PaymentStatus PaymentStatus = PaymentStatus.Rejected;
        public ValidationResult ValidationResult { get; init; } = validationResult;
    }
}