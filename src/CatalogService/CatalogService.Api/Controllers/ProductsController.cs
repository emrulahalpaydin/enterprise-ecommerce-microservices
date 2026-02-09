using CatalogService.Application.Commands;
using CatalogService.Application.DTOs;
using CatalogService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public Task<IReadOnlyCollection<ProductDto>> List() => _mediator.Send(new ListProductsQuery());

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public Task<ProductDto?> Get(Guid id) => _mediator.Send(new GetProductQuery(id));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public Task<ProductDto> Create([FromBody] CreateProductCommand cmd) => _mediator.Send(cmd);

    [HttpPost("{id:guid}/stock")]
    [Authorize(Roles = "Admin")]
    public Task<ProductDto> UpdateStock(Guid id, [FromBody] int delta)
        => _mediator.Send(new UpdateStockCommand(id, delta));
}
