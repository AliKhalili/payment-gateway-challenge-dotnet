namespace PaymentGateway.Api.Infrastructures.MockBank
{
    public class MockBankClient : IMockBankClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MockBankClient> _logger;

        public MockBankClient(HttpClient httpClient, ILogger<MockBankClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<MockBankPaymentResponse?> ProcessPaymentAsync(MockBankPaymentRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var response = await _httpClient.PostAsJsonAsync("payments", request, cancellationToken);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<MockBankPaymentResponse>(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment request to MockBank");
                throw new MockBankProcessPaymentException(ex);
            }
        }
    }
}