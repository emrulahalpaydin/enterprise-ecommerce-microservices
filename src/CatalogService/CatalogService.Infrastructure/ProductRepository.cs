using CatalogService.Application;
using CatalogService.Domain;
using CatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _db;
    public ProductRepository(CatalogDbContext db) => _db = db;

    public Task AddAsync(Product p)
    {
        var entity = new ProductEntity
        {
            Id = p.Id,
            Name = p.Name,
            SKU = p.Sku.Value,
            Description = p.Description,
            Price = p.Price.Amount,
            Currency = p.Price.Currency,
            Stock = p.Stock,
            CategoryId = p.CategoryId
        };

        foreach (var v in p.Variants)
        {
            entity.Variants.Add(new ProductVariantEntity
            {
                Id = Guid.NewGuid(),
                ProductId = p.Id,
                Name = v.Name,
                Value = v.Value
            });
        }

        _db.Products.Add(entity);
        return Task.CompletedTask;
    }

    public IQueryable<Product> Query()
    {
        return _db.Products.Include(p => p.Variants).Select(e =>
            Product.Load(
                e.Id,
                e.Name,
                new Sku(e.SKU),
                e.Description,
                new Money(e.Price, e.Currency),
                e.Stock,
                e.CategoryId,
                e.Variants.Select(v => new ProductVariant(v.Name, v.Value))));
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var e = await _db.Products.Include(p => p.Variants).FirstOrDefaultAsync(p => p.Id == id);
        if (e == null) return null;
        return Product.Load(
            e.Id,
            e.Name,
            new Sku(e.SKU),
            e.Description,
            new Money(e.Price, e.Currency),
            e.Stock,
            e.CategoryId,
            e.Variants.Select(v => new ProductVariant(v.Name, v.Value)));
    }
}
