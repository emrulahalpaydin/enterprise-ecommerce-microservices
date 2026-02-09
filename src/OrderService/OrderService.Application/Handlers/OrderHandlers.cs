using MediatR;
using Microservices.Shared.BuildingBlocks.Application;
using Microservices.Shared.Contracts;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain;

namespace OrderService.Application.Handlers;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orders;
    private readonly IIntegrationEventPublisher _publisher;

    public CreateOrderHandler(IOrderRepository orders, IIntegrationEventPublisher publisher)
    {
        _orders = orders;
        _publisher = publisher;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new OrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity));
        var order = Order.Create(Guid.NewGuid(), request.UserId, items);
        await _orders.AddAsync(order);
        await _orders.SaveChangesAsync();

        await _publisher.PublishAsync(new OrderCreatedEvent(order.Id, order.UserId, order.Total));

        return Map(order);
    }

    private static OrderDto Map(Order o) => new(
        o.Id,
        o.UserId,
        o.Total,
        o.Status.ToString(),
        o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
}

public sealed class MarkOrderPaidHandler : IRequestHandler<MarkOrderPaidCommand, OrderDto>
{
    private readonly IOrderRepository _orders;
    public MarkOrderPaidHandler(IOrderRepository orders) => _orders = orders;

    public async Task<OrderDto> Handle(MarkOrderPaidCommand request, CancellationToken cancellationToken)
    {
        var updated = await _orders.MarkPaidAsync(request.OrderId);
        if (!updated) throw new InvalidOperationException("Order not found");
        await _orders.SaveChangesAsync();
        var order = await _orders.GetByIdAsync(request.OrderId);
        if (order == null) throw new InvalidOperationException("Order not found");
        return new OrderDto(order.Id, order.UserId, order.Total, order.Status.ToString(), order.CreatedAt, order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
    }
}

public sealed class MarkOrderFailedHandler : IRequestHandler<MarkOrderFailedCommand, OrderDto>
{
    private readonly IOrderRepository _orders;
    public MarkOrderFailedHandler(IOrderRepository orders) => _orders = orders;

    public async Task<OrderDto> Handle(MarkOrderFailedCommand request, CancellationToken cancellationToken)
    {
        var updated = await _orders.MarkFailedAsync(request.OrderId);
        if (!updated) throw new InvalidOperationException("Order not found");
        await _orders.SaveChangesAsync();
        var order = await _orders.GetByIdAsync(request.OrderId);
        if (order == null) throw new InvalidOperationException("Order not found");
        return new OrderDto(order.Id, order.UserId, order.Total, order.Status.ToString(), order.CreatedAt, order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
    }
}

public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDto?>
{
    private readonly IOrderRepository _orders;
    public GetOrderHandler(IOrderRepository orders) => _orders = orders;

    public async Task<OrderDto?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(request.Id);
        return order == null ? null : new OrderDto(order.Id, order.UserId, order.Total, order.Status.ToString(), order.CreatedAt, order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());
    }
}

public sealed class ListOrdersHandler : IRequestHandler<ListOrdersQuery, IReadOnlyCollection<OrderDto>>
{
    private readonly IOrderRepository _orders;
    public ListOrdersHandler(IOrderRepository orders) => _orders = orders;

    public Task<IReadOnlyCollection<OrderDto>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var data = _orders.Query().Select(o => new OrderDto(o.Id, o.UserId, o.Total, o.Status.ToString(), o.CreatedAt, o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList())).ToList();
        return Task.FromResult<IReadOnlyCollection<OrderDto>>(data);
    }
}
