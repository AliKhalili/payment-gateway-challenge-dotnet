using PaymentGateway.Api.Infrastructures.MockBank;

namespace PaymentGateway.Api.Infrastructures
{
    public interface IMockBankClient
    {
        Task<MockBankPaymentResponse?> ProcessPaymentAsync(MockBankPaymentRequest request, CancellationToken cancellationToken);
    }
}