using System.Collections.Concurrent;

namespace AirsoftEvents.Api.Payments;

public enum MockMolliePaymentStatus
{
    Open,
    Paid,
    Failed,
    Canceled,
    Expired
}

public record MockMolliePayment(string PaymentId, Guid ReservationId, MockMolliePaymentStatus Status);

public class MockMollieStore
{
    private readonly ConcurrentDictionary<string, MockMolliePayment> _payments = new();

    public MockMolliePayment Create(Guid reservationId)
    {
        var paymentId = "tr_" + Guid.NewGuid().ToString("N");
        var payment = new MockMolliePayment(paymentId, reservationId, MockMolliePaymentStatus.Open);
        _payments[paymentId] = payment;
        return payment;
    }

    public MockMolliePayment? Get(string paymentId)
        => _payments.TryGetValue(paymentId, out var p) ? p : null;

    public bool UpdateStatus(string paymentId, MockMolliePaymentStatus status)
    {
        if (!_payments.TryGetValue(paymentId, out var p)) return false;
        _payments[paymentId] = p with { Status = status };
        return true;
    }
}
