namespace PaymentGateway.Api.Models.Responses;

public class GetPaymentResponse
{
    public string Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public long Amount { get; set; }
}