using Microservices.Shared.BuildingBlocks.Domain;

namespace PaymentService.Domain;

public sealed record PaymentCompletedDomainEvent(Guid PaymentId, Guid OrderId, decimal Amount) : DomainEvent;
public sealed record PaymentFailedDomainEvent(Guid PaymentId, Guid OrderId, decimal Amount, string Reason) : DomainEvent;
