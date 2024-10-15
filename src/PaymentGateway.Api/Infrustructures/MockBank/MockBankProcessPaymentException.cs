namespace PaymentGateway.Api.Infrastructures.MockBank
{
    public class MockBankProcessPaymentException : Exception
    {
        public MockBankProcessPaymentException(Exception exception) : base("Mock Bank Exception", exception) { }
    }
}