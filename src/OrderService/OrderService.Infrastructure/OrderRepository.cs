using OrderService.Application;
using OrderService.Domain;
using OrderService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _db;
    public OrderRepository(OrderDbContext db) => _db = db;

    public Task AddAsync(Order o)
    {
        var orderEntity = new OrderEntity { Id = o.Id, UserId = o.UserId, Total = o.Total, Status = o.Status.ToString(), CreatedAt = o.CreatedAt };
        foreach (var item in o.Items)
        {
            orderEntity.Items.Add(new OrderItemEntity { Id = Guid.NewGuid(), ProductId = item.ProductId, ProductName = item.ProductName, UnitPrice = item.UnitPrice, Quantity = item.Quantity });
        }
        _db.Orders.Add(orderEntity);
        return Task.CompletedTask;
    }

    public IQueryable<Order> Query()
    {
        return _db.Orders.Include(o => o.Items).AsEnumerable().Select(e =>
            Order.Load(
                e.Id,
                e.UserId,
                e.Total,
                Enum.TryParse<OrderStatus>(e.Status, out var s) ? s : OrderStatus.Pending,
                e.CreatedAt,
                e.Items.Select(i => new OrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity))
            )).AsQueryable();
    }

    public async Task<bool> MarkPaidAsync(Guid id)
    {
        var e = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (e == null) return false;
        e.Status = OrderStatus.Paid.ToString();
        return true;
    }

    public async Task<bool> MarkFailedAsync(Guid id)
    {
        var e = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (e == null) return false;
        e.Status = OrderStatus.Failed.ToString();
        return true;
    }

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        var e = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (e == null) return null;
        return Order.Load(
            e.Id,
            e.UserId,
            e.Total,
            Enum.TryParse<OrderStatus>(e.Status, out var s) ? s : OrderStatus.Pending,
            e.CreatedAt,
            e.Items.Select(i => new OrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity))
        );
    }
}
