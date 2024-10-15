using System.Collections.Concurrent;

namespace PaymentGateway.Api.Infrastructures;

public class PaymentsRepository
{
    public ConcurrentDictionary<string, StorePaymentModel> _db = new();

    public void Add(StorePaymentModel payment)
    {
        _db.TryAdd(payment.PaymentId, payment);
    }

    public StorePaymentModel? Get(string id)
    {
        _db.TryGetValue(id, out var model);
        return model;
    }
}