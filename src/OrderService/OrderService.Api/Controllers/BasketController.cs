using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IMediator _mediator;
    public BasketController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{userId:guid}")]
    [Authorize]
    public Task<BasketDto?> Get(Guid userId) => _mediator.Send(new GetBasketQuery(userId));

    [HttpPost("{userId:guid}/items")]
    [Authorize]
    public Task<BasketDto> AddItem(Guid userId, [FromBody] BasketItemDto item)
        => _mediator.Send(new AddBasketItemCommand(userId, item));

    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    [Authorize]
    public Task<BasketDto> RemoveItem(Guid userId, Guid productId)
        => _mediator.Send(new RemoveBasketItemCommand(userId, productId));

    [HttpDelete("{userId:guid}")]
    [Authorize]
    public Task<BasketDto> Clear(Guid userId)
        => _mediator.Send(new ClearBasketCommand(userId));
}
