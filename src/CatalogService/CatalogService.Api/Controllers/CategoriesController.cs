using CatalogService.Application.Commands;
using CatalogService.Application.DTOs;
using CatalogService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public Task<IReadOnlyCollection<CategoryDto>> List() => _mediator.Send(new ListCategoriesQuery());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public Task<CategoryDto> Create([FromBody] CreateCategoryCommand cmd) => _mediator.Send(cmd);
}
