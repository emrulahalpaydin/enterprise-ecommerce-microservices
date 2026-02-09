using Microsoft.EntityFrameworkCore;
using OrderService.Application;
using OrderService.Domain;
using OrderService.Infrastructure.Entities;

namespace OrderService.Infrastructure;

public class BasketRepository : IBasketRepository
{
    private readonly OrderDbContext _db;
    public BasketRepository(OrderDbContext db) => _db = db;

    public async Task<Basket?> GetByUserIdAsync(Guid userId)
    {
        var e = await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.UserId == userId);
        if (e == null) return null;
        return Basket.Load(e.Id, e.UserId, e.CreatedAt, e.Items.Select(i => new BasketItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)));
    }

    public Task AddAsync(Basket basket)
    {
        var entity = new BasketEntity { Id = basket.Id, UserId = basket.UserId, CreatedAt = basket.CreatedAt };
        foreach (var i in basket.Items)
        {
            entity.Items.Add(new BasketItemEntity
            {
                Id = Guid.NewGuid(),
                BasketId = entity.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            });
        }
        _db.Baskets.Add(entity);
        return Task.CompletedTask;
    }

    public async Task UpsertAsync(Basket basket)
    {
        var existing = await _db.Baskets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == basket.Id);
        if (existing == null)
        {
            await AddAsync(basket);
            return;
        }

        existing.Items.Clear();
        foreach (var i in basket.Items)
        {
            existing.Items.Add(new BasketItemEntity
            {
                Id = Guid.NewGuid(),
                BasketId = existing.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            });
        }
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
