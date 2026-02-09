using CatalogService.Domain;

namespace CatalogService.Application;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<Category?> GetByIdAsync(Guid id);
    IQueryable<Category> Query();
    Task SaveChangesAsync();
}
