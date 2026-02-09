using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands;

public sealed record CreateOrderCommand(Guid UserId, IReadOnlyCollection<OrderItemDto> Items) : IRequest<OrderDto>;
public sealed record MarkOrderPaidCommand(Guid OrderId) : IRequest<OrderDto>;
public sealed record MarkOrderFailedCommand(Guid OrderId, string Reason) : IRequest<OrderDto>;
