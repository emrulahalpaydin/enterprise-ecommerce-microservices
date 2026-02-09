using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize]
    public Task<IReadOnlyCollection<OrderDto>> List() => _mediator.Send(new ListOrdersQuery());

    [HttpGet("{id:guid}")]
    [Authorize]
    public Task<OrderDto?> Get(Guid id) => _mediator.Send(new GetOrderQuery(id));

    [HttpPost]
    [Authorize]
    public Task<OrderDto> Create([FromBody] CreateOrderCommand cmd) => _mediator.Send(cmd);
}
