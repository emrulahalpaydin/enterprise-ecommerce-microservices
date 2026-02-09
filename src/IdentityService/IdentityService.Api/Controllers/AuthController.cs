using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [AllowAnonymous]
    public Task<AuthResponse> Register([FromBody] RegisterUserCommand cmd) => _mediator.Send(cmd);

    [HttpPost("login")]
    [AllowAnonymous]
    public Task<AuthResponse> Login([FromBody] LoginCommand cmd) => _mediator.Send(cmd);

    [HttpPost("refresh")]
    [AllowAnonymous]
    public Task<AuthResponse> Refresh([FromBody] RefreshTokenCommand cmd) => _mediator.Send(cmd);

    [HttpPost("bootstrap-admin")]
    [AllowAnonymous]
    public Task<AuthResponse> BootstrapAdmin([FromBody] BootstrapAdminCommand cmd) => _mediator.Send(cmd);

    [HttpPost("users/{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public Task<UserDto> ChangeRole(Guid id, [FromBody] ChangeRoleRequest body)
        => _mediator.Send(new ChangeUserRoleCommand(id, body.Role));
}

public sealed record ChangeRoleRequest(string Role);
