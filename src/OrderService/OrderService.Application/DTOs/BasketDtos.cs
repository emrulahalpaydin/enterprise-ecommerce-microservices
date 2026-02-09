namespace OrderService.Application.DTOs;

public sealed record BasketItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);
public sealed record BasketDto(Guid Id, Guid UserId, IReadOnlyCollection<BasketItemDto> Items);
