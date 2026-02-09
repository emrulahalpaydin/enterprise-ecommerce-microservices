using Microservices.Shared.BuildingBlocks.Domain;

namespace CatalogService.Domain;

public sealed class Product : AggregateRoot
{
    private readonly List<ProductVariant> _variants = new();

    private Product() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Sku Sku { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = new Money(0, "USD");
    public int Stock { get; private set; }
    public Guid CategoryId { get; private set; }

    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    public static Product Create(Guid id, string name, Sku sku, string description, Money price, int stock, Guid categoryId)
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            Sku = sku,
            Description = description,
            Price = price,
            Stock = stock,
            CategoryId = categoryId
        };

        product.AddDomainEvent(new ProductCreatedDomainEvent(id, name, sku.Value, stock));
        return product;
    }

    public static Product Load(Guid id, string name, Sku sku, string description, Money price, int stock, Guid categoryId, IEnumerable<ProductVariant> variants)
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            Sku = sku,
            Description = description,
            Price = price,
            Stock = stock,
            CategoryId = categoryId
        };
        foreach (var v in variants)
            product._variants.Add(v);
        return product;
    }

    public void AdjustStock(int delta)
    {
        var newStock = Stock + delta;
        if (newStock < 0) throw new InvalidOperationException("Insufficient stock");
        Stock = newStock;
    }

    public void AddVariant(ProductVariant variant)
    {
        _variants.Add(variant);
    }
}
