using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries;

public sealed record GetOrderQuery(Guid Id) : IRequest<OrderDto?>;
public sealed record ListOrdersQuery() : IRequest<IReadOnlyCollection<OrderDto>>;
