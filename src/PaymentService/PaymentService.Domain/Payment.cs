using Microservices.Shared.BuildingBlocks.Domain;

namespace PaymentService.Domain;

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}

public sealed class Payment : AggregateRoot
{
    private Payment() { }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? Method { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Payment Create(Guid id, Guid orderId, decimal amount, string? method)
    {
        return new Payment
        {
            Id = id,
            OrderId = orderId,
            Amount = amount,
            Method = method,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Payment Load(Guid id, Guid orderId, decimal amount, PaymentStatus status, string? method, string? transactionId, DateTime createdAt)
    {
        return new Payment
        {
            Id = id,
            OrderId = orderId,
            Amount = amount,
            Status = status,
            Method = method,
            TransactionId = transactionId,
            CreatedAt = createdAt
        };
    }

    public void MarkCompleted(string transactionId)
    {
        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        AddDomainEvent(new PaymentCompletedDomainEvent(Id, OrderId, Amount));
    }

    public void MarkFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        AddDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, Amount, reason));
    }
}
