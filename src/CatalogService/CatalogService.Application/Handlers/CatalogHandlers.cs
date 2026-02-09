using CatalogService.Application.Commands;
using CatalogService.Application.DTOs;
using CatalogService.Application.Queries;
using CatalogService.Domain;
using MediatR;
using Microservices.Shared.BuildingBlocks.Application;
using Microservices.Shared.Contracts;

namespace CatalogService.Application.Handlers;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;
    private readonly IIntegrationEventPublisher _publisher;

    public CreateProductHandler(IProductRepository products, ICategoryRepository categories, IIntegrationEventPublisher publisher)
    {
        _products = products;
        _categories = categories;
        _publisher = publisher;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _categories.GetByIdAsync(request.CategoryId);
        if (category == null) throw new InvalidOperationException("Category not found");

        var product = Product.Create(
            Guid.NewGuid(),
            request.Name,
            new Sku(request.Sku),
            request.Description,
            new Money(request.Price, request.Currency),
            request.Stock,
            request.CategoryId);

        foreach (var v in request.Variants ?? Array.Empty<ProductVariantDto>())
        {
            product.AddVariant(new ProductVariant(v.Name, v.Value));
        }

        await _products.AddAsync(product);
        await _products.SaveChangesAsync();

        await _publisher.PublishAsync(new ProductCreatedEvent(product.Id, product.Name, product.Sku.Value, product.Stock));

        return Map(product);
    }

    private static ProductDto Map(Product p) => new(
        p.Id,
        p.Name,
        p.Sku.Value,
        p.Description,
        p.Price.Amount,
        p.Price.Currency,
        p.Stock,
        p.CategoryId,
        p.Variants.Select(v => new ProductVariantDto(v.Name, v.Value)).ToList());
}

public sealed class UpdateStockHandler : IRequestHandler<UpdateStockCommand, ProductDto>
{
    private readonly IProductRepository _products;

    public UpdateStockHandler(IProductRepository products) => _products = products;

    public async Task<ProductDto> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _products.GetByIdAsync(request.ProductId);
        if (product == null) throw new InvalidOperationException("Product not found");
        product.AdjustStock(request.Delta);
        await _products.SaveChangesAsync();
        return new ProductDto(product.Id, product.Name, product.Sku.Value, product.Description, product.Price.Amount, product.Price.Currency, product.Stock, product.CategoryId, product.Variants.Select(v => new ProductVariantDto(v.Name, v.Value)).ToList());
    }
}

public sealed class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categories;
    public CreateCategoryHandler(ICategoryRepository categories) => _categories = categories;

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(Guid.NewGuid(), request.Name, request.Description);
        await _categories.AddAsync(category);
        await _categories.SaveChangesAsync();
        return new CategoryDto(category.Id, category.Name, category.Description);
    }
}

public sealed class GetProductHandler : IRequestHandler<GetProductQuery, ProductDto?>
{
    private readonly IProductRepository _products;
    public GetProductHandler(IProductRepository products) => _products = products;

    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _products.GetByIdAsync(request.Id);
        return product == null ? null : new ProductDto(product.Id, product.Name, product.Sku.Value, product.Description, product.Price.Amount, product.Price.Currency, product.Stock, product.CategoryId, product.Variants.Select(v => new ProductVariantDto(v.Name, v.Value)).ToList());
    }
}

public sealed class ListProductsHandler : IRequestHandler<ListProductsQuery, IReadOnlyCollection<ProductDto>>
{
    private readonly IProductRepository _products;
    public ListProductsHandler(IProductRepository products) => _products = products;

    public Task<IReadOnlyCollection<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var data = _products.Query()
            .AsEnumerable()
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Sku.Value,
                p.Description,
                p.Price.Amount,
                p.Price.Currency,
                p.Stock,
                p.CategoryId,
                p.Variants.Select(v => new ProductVariantDto(v.Name, v.Value)).ToList()))
            .ToList();
        return Task.FromResult<IReadOnlyCollection<ProductDto>>(data);
    }
}

public sealed class ListCategoriesHandler : IRequestHandler<ListCategoriesQuery, IReadOnlyCollection<CategoryDto>>
{
    private readonly ICategoryRepository _categories;
    public ListCategoriesHandler(ICategoryRepository categories) => _categories = categories;

    public Task<IReadOnlyCollection<CategoryDto>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        var data = _categories.Query().Select(c => new CategoryDto(c.Id, c.Name, c.Description)).ToList();
        return Task.FromResult<IReadOnlyCollection<CategoryDto>>(data);
    }
}
