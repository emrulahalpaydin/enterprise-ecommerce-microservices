using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Sku,
    string Description,
    decimal Price,
    string Currency,
    int Stock,
    Guid CategoryId,
    IReadOnlyCollection<ProductVariantDto> Variants) : IRequest<ProductDto>;

public sealed record UpdateStockCommand(Guid ProductId, int Delta) : IRequest<ProductDto>;

public sealed record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;
