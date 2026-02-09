using Microservices.Shared.BuildingBlocks.Domain;

namespace IdentityService.Domain;

public sealed record UserRegisteredDomainEvent(Guid UserId, string Email, string Role) : DomainEvent;
