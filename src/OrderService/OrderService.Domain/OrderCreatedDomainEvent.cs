using Microservices.Shared.BuildingBlocks.Domain;

namespace OrderService.Domain;

public sealed record OrderCreatedDomainEvent(Guid OrderId, Guid UserId, decimal Total) : DomainEvent;
