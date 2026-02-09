using System;

namespace Microservices.Shared.BuildingBlocks.Domain;

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
