using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries;

public sealed record GetBasketQuery(Guid UserId) : IRequest<BasketDto?>;
