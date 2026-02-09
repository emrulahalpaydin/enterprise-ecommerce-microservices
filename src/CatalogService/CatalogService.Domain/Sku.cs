using Microservices.Shared.BuildingBlocks.Domain;

namespace CatalogService.Domain;

public sealed class Sku : ValueObject
{
    public string Value { get; }

    public Sku(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU is required", nameof(value));
        Value = value.Trim().ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
