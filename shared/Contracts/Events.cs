using System;

namespace Microservices.Shared.Contracts
{
    public record UserRegisteredEvent(Guid UserId, string Email, string Role);

    public record ProductCreatedEvent(Guid ProductId, string Name, string SKU, int Stock);

    public record OrderCreatedEvent(Guid OrderId, Guid UserId, decimal Total);

    public record PaymentCompletedEvent(Guid PaymentId, Guid OrderId, decimal Amount);

    public record PaymentFailedEvent(Guid PaymentId, Guid OrderId, decimal Amount, string Reason);
}
