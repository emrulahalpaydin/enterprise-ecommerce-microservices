using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands;

public sealed record AddBasketItemCommand(Guid UserId, BasketItemDto Item) : IRequest<BasketDto>;
public sealed record RemoveBasketItemCommand(Guid UserId, Guid ProductId) : IRequest<BasketDto>;
public sealed record ClearBasketCommand(Guid UserId) : IRequest<BasketDto>;
