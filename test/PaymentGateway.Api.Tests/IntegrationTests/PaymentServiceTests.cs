using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

using Moq;

using PaymentGateway.Api.Applications.Payment;
using PaymentGateway.Api.Infrastructures;
using PaymentGateway.Api.Infrastructures.MockBank;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Validations;

namespace PaymentGateway.Api.Tests.IntegrationTests
{
    public class PaymentServiceTests
    {

        private readonly PaymentsRepository _paymentsRepository;
        private readonly Mock<IMockBankClient> _mockBankClientMock;
        private readonly PaymentService _paymentService;

        public PaymentServiceTests()
        {
            _paymentsRepository = new PaymentsRepository();
            _mockBankClientMock = new Mock<IMockBankClient>();

            _paymentService = new PaymentService(
                _paymentsRepository,
                new PaymentRequestValidator(new FakeTimeProvider(new DateTime(2024, 02, 01))),
                _mockBankClientMock.Object,
                NullLogger<PaymentService>.Instance);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnProcessPaymentDone_WhenPaymentIsAuthorized()
        {
            // Arrange
            var paymentId = "test_payment_id";
            var paymentRequest = new PaymentRequest
            {
                Amount = 100,
                Currency = Enums.PaymentCurrency.USD,
                CardNumber = "2222405343248877",
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025
            };
            var cancellationToken = CancellationToken.None;

            _mockBankClientMock.Setup(m => m.ProcessPaymentAsync(It.IsAny<MockBankPaymentRequest>(), cancellationToken))
                .ReturnsAsync(new MockBankPaymentResponse { Authorized = true });

            // Act
            var result = await _paymentService.ProcessPaymentAsync(paymentId, paymentRequest, cancellationToken);

            // Assert
            var paymentDone = result.AsT1;
            paymentDone.PaymentId.Should().Be(paymentId);
            paymentDone.PaymentStatus.Should().Be(PaymentStatus.Authorized);

            var storePayment = _paymentsRepository.Get(paymentId);
            storePayment.Should().NotBeNull();
            storePayment!.Status.Should().Be(PaymentStatus.Authorized);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnProcessPaymentRejected_WhenValidationFails()
        {
            // Arrange
            var paymentId = "test_payment_id";
            var paymentRequest = new PaymentRequest
            {
                Amount = -100, // Invalid amount
                Currency = Enums.PaymentCurrency.USD,
                CardNumber = "2222405343248877",
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025
            };
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _paymentService.ProcessPaymentAsync(paymentId, paymentRequest, cancellationToken);

            // Assert
            result.IsT0.Should().BeTrue();
            var rejected = result.AsT0;
            rejected.PaymentId.Should().Be(paymentId);
            rejected.ValidationResult.Errors.Should().ContainSingle(e => e.PropertyName == "Amount");

            var storePayment = _paymentsRepository.Get(paymentId);
            storePayment.Should().BeNull();
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnProcessPaymentRejected_WhenPaymentIsDeclined()
        {
            // Arrange
            var paymentId = "test_payment_id";
            var paymentRequest = new PaymentRequest
            {
                Amount = 100,
                Currency = Enums.PaymentCurrency.USD,
                CardNumber = "2222405343248877",
                Cvv = "123",
                ExpiryMonth = 4,
                ExpiryYear = 2025
            };
            var cancellationToken = CancellationToken.None;

            _mockBankClientMock.Setup(m => m.ProcessPaymentAsync(It.IsAny<MockBankPaymentRequest>(), cancellationToken))
                .ReturnsAsync(new MockBankPaymentResponse { Authorized = false });

            // Act
            var result = await _paymentService.ProcessPaymentAsync(paymentId, paymentRequest, cancellationToken);

            // Assert
            result.IsT1.Should().BeTrue();
            var rejected = result.AsT1;
            rejected.PaymentId.Should().Be(paymentId);

            var storePayment = _paymentsRepository.Get(paymentId);
            storePayment.Should().NotBeNull();
            storePayment!.Status.Should().Be(PaymentStatus.Declined);
        }
    }
}