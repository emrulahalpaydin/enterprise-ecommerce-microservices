using OrderService.Domain;

namespace OrderService.Application;

public interface IBasketRepository
{
    Task<Basket?> GetByUserIdAsync(Guid userId);
    Task AddAsync(Basket basket);
    Task UpsertAsync(Basket basket);
    Task SaveChangesAsync();
}
