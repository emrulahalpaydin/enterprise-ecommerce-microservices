using CatalogService.Domain;

namespace CatalogService.Application;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    IQueryable<Product> Query();
    Task SaveChangesAsync();
}
