using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Queries;

public sealed record GetProductQuery(Guid Id) : IRequest<ProductDto?>;
public sealed record ListProductsQuery() : IRequest<IReadOnlyCollection<ProductDto>>;
public sealed record ListCategoriesQuery() : IRequest<IReadOnlyCollection<CategoryDto>>;
