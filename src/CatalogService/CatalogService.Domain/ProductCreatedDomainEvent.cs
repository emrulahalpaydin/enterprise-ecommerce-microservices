using Microservices.Shared.BuildingBlocks.Domain;

namespace CatalogService.Domain;

public sealed record ProductCreatedDomainEvent(Guid ProductId, string Name, string Sku, int Stock) : DomainEvent;
