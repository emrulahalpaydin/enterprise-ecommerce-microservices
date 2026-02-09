using Microservices.Shared.BuildingBlocks.Domain;

namespace OrderService.Domain;

public sealed class Basket : AggregateRoot
{
    private readonly List<BasketItem> _items = new();

    private Basket() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public static Basket Create(Guid id, Guid userId)
    {
        return new Basket { Id = id, UserId = userId, CreatedAt = DateTime.UtcNow };
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            _items.Remove(existing);
            _items.Add(existing.WithQuantity(existing.Quantity + quantity));
            return;
        }

        _items.Add(new BasketItem(productId, productName, unitPrice, quantity));
    }

    public void RemoveItem(Guid productId)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null) _items.Remove(existing);
    }

    public void Clear() => _items.Clear();

    public static Basket Load(Guid id, Guid userId, DateTime createdAt, IEnumerable<BasketItem> items)
    {
        var b = new Basket { Id = id, UserId = userId, CreatedAt = createdAt };
        foreach (var i in items) b._items.Add(i);
        return b;
    }
}

public sealed class BasketItem : ValueObject
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; }

    public BasketItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public BasketItem WithQuantity(int quantity) => new(ProductId, ProductName, UnitPrice, quantity);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return UnitPrice;
        yield return Quantity;
    }
}
