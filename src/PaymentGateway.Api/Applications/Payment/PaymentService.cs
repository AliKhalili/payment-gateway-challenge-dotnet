using FluentValidation;

using OneOf;

using PaymentGateway.Api.Infrastructures;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Validations;

namespace PaymentGateway.Api.Applications.Payment
{
    public class PaymentService
    {
        private readonly PaymentsRepository _paymentsRepository;
        private readonly IValidator<PaymentRequest> _paymentRequestValidator;
        private readonly IMockBankClient _mockBankClient;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(PaymentsRepository paymentsRepository, IValidator<PaymentRequest> paymentRequestValidator, IMockBankClient mockBankClient, ILogger<PaymentService> logger)
        {
            _paymentsRepository = paymentsRepository;
            _paymentRequestValidator = paymentRequestValidator;
            _mockBankClient = mockBankClient;
            _logger = logger;
        }

        /// <summary>
        /// Checks if a payment with the given Id is a duplicate. 
        /// Returns false if it exists, the original Id if not, or a new payment Id if the Id is null/empty.
        /// </summary>
        /// <param name="paymentId">The payment ID to check.</param>
        /// <returns>
        /// A <see cref="OneOf{T0, T1}"/>:
        /// - <c>false</c> if the payment exists.
        /// - The original Id or a new id string in case of null.
        /// </returns>
        public OneOf<bool, string> IsPaymentDuplicate(string? paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
            {
                return Guid.NewGuid().ToString();
            }
            var existingPayment = _paymentsRepository.Get(paymentId);
            if (existingPayment is not null && existingPayment.PaymentId.Equals(paymentId, StringComparison.Ordinal))
            {
                return false;
            }
            return paymentId;
        }

        public GetPaymentResponse? GetPayment(string paymentId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);

            var storeModel = _paymentsRepository.Get(paymentId);
            if (storeModel is null)
            {
                return null;
            }

            return storeModel.ToGetPaymentResponse();
        }

        public async Task<OneOf<ProcessPaymentRejected, ProcessPaymentDone>> ProcessPaymentAsync(string paymentId, PaymentRequest paymentRequest, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var validationResult = _paymentRequestValidator.Validate(paymentRequest);
                if (!validationResult.IsValid)
                {
                    return new ProcessPaymentRejected(paymentId, validationResult);
                }


                StorePaymentModel storeModel;

                var paymentResult = await _mockBankClient.ProcessPaymentAsync(paymentRequest.ToMockBankRequest(), cancellationToken);
                if (paymentResult is null || !paymentResult.Authorized)
                {
                    storeModel = new StorePaymentModel(paymentId, PaymentStatus.Declined, paymentRequest);
                }
                else
                {
                    storeModel = new StorePaymentModel(paymentId, PaymentStatus.Authorized, paymentRequest);
                }

                _paymentsRepository.Add(storeModel);
                _logger.LogInformation("New payment request with payment id {paymentId} executed.", paymentId);

                return new ProcessPaymentDone(paymentId, storeModel.Status, storeModel.ToPostPaymentResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for payment id {paymentId}", paymentId);
                return new ProcessPaymentRejected(paymentId, ex.CreateValidationResult());
            }
        }
    }
}