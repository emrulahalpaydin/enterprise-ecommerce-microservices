using OrderService.Domain;

namespace OrderService.Application;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    IQueryable<Order> Query();
    Task<bool> MarkPaidAsync(Guid id);
    Task<bool> MarkFailedAsync(Guid id);
    Task SaveChangesAsync();
}
