using Microservices.Shared.BuildingBlocks.Domain;

namespace OrderService.Domain;

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled,
    Failed
}

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    private Order() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(Guid id, Guid userId, IEnumerable<OrderItem> items)
    {
        var order = new Order
        {
            Id = id,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in items)
        {
            order._items.Add(item);
        }

        order.Total = order._items.Sum(i => i.UnitPrice * i.Quantity);
        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, order.UserId, order.Total));
        return order;
    }

    public static Order Load(Guid id, Guid userId, decimal total, OrderStatus status, DateTime createdAt, IEnumerable<OrderItem> items)
    {
        var order = new Order
        {
            Id = id,
            UserId = userId,
            Total = total,
            Status = status,
            CreatedAt = createdAt
        };
        foreach (var item in items)
            order._items.Add(item);
        return order;
    }

    public void MarkPaid()
    {
        Status = OrderStatus.Paid;
    }

    public void MarkFailed()
    {
        Status = OrderStatus.Failed;
    }
}

public sealed class OrderItem : ValueObject
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }

    public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return UnitPrice;
        yield return Quantity;
    }
}
