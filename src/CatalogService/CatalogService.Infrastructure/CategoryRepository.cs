using CatalogService.Application;
using CatalogService.Domain;
using CatalogService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure;

public class CategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext _db;
    public CategoryRepository(CatalogDbContext db) => _db = db;

    public Task AddAsync(Category category)
    {
        _db.Categories.Add(new CategoryEntity { Id = category.Id, Name = category.Name, Description = category.Description });
        return Task.CompletedTask;
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        var e = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return e == null ? null : Category.Create(e.Id, e.Name, e.Description);
    }

    public IQueryable<Category> Query()
    {
        return _db.Categories.AsNoTracking().Select(c => Category.Create(c.Id, c.Name, c.Description));
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
