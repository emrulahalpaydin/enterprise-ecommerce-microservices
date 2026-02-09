using Microservices.Shared.BuildingBlocks.Domain;

namespace CatalogService.Domain;

public sealed class ProductVariant : ValueObject
{
    public string Name { get; }
    public string Value { get; }

    public ProductVariant(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Variant name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Variant value is required", nameof(value));
        Name = name.Trim();
        Value = value.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }
}
