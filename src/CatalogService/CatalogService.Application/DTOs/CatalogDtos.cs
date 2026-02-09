namespace CatalogService.Application.DTOs;

public sealed record ProductVariantDto(string Name, string Value);

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    string Description,
    decimal Price,
    string Currency,
    int Stock,
    Guid CategoryId,
    IReadOnlyCollection<ProductVariantDto> Variants);

public sealed record CategoryDto(Guid Id, string Name, string? Description);
