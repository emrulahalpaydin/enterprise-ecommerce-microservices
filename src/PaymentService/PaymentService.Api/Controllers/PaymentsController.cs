using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Commands;
using PaymentService.Application.DTOs;
using PaymentService.Application.Queries;

namespace PaymentService.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize]
    public Task<IReadOnlyCollection<PaymentDto>> List() => _mediator.Send(new ListPaymentsQuery());

    [HttpGet("{id:guid}")]
    [Authorize]
    public Task<PaymentDto?> Get(Guid id) => _mediator.Send(new GetPaymentQuery(id));

    [HttpPost]
    [Authorize]
    public Task<PaymentDto> Create([FromBody] CreatePaymentCommand cmd) => _mediator.Send(cmd);
}
