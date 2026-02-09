using Microservices.Shared.BuildingBlocks.Domain;

namespace CatalogService.Domain;

public sealed class Category : Entity
{
    private Category() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public static Category Create(Guid id, string name, string? description)
    {
        return new Category { Id = id, Name = name, Description = description };
    }
}
