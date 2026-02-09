using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Commands;

public sealed record RegisterUserCommand(string Email, string Password) : IRequest<AuthResponse>;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponse>;

public sealed record ChangeUserRoleCommand(Guid UserId, string Role) : IRequest<UserDto>;

public sealed record BootstrapAdminCommand(string Email, string Password, string Secret) : IRequest<AuthResponse>;
