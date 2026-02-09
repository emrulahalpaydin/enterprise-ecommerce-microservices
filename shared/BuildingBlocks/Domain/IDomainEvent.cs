using System;

namespace Microservices.Shared.BuildingBlocks.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
