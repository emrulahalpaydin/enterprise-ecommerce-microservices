namespace OrderService.Application.DTOs;

public sealed record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);

public sealed record OrderDto(Guid Id, Guid UserId, decimal Total, string Status, DateTime CreatedAt, IReadOnlyCollection<OrderItemDto> Items);
