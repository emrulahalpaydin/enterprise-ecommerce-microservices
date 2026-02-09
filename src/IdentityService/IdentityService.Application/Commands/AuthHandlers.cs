using IdentityService.Application.Commands;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain;
using MediatR;
using Microservices.Shared.BuildingBlocks.Application;
using Microservices.Shared.Contracts;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handlers;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IIntegrationEventPublisher _publisher;

    public RegisterUserHandler(IUserRepository users, IPasswordHasher hasher, ITokenService tokens, IRefreshTokenRepository refreshTokens, IIntegrationEventPublisher publisher)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _refreshTokens = refreshTokens;
        _publisher = publisher;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var existing = await _users.GetByEmailAsync(email);
        if (existing != null) throw new InvalidOperationException("Email already registered");

        var id = Guid.NewGuid();
        var hash = _hasher.Hash(request.Password);
        var user = User.Register(id, email, hash, Roles.User);

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        await _publisher.PublishAsync(new UserRegisteredEvent(user.Id, user.Email.Value, user.Role));

        var accessToken = _tokens.CreateAccessToken(user.Id, user.Email.Value, user.Role);
        var refresh = _tokens.CreateRefreshToken();
        await _refreshTokens.SaveAsync(user.Id, refresh.Token, refresh.ExpiresAt);

        return new AuthResponse(accessToken, refresh.Token, refresh.ExpiresAt, new UserDto(user.Id, user.Email.Value, user.Role));
    }
}

public sealed class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IRefreshTokenRepository _refreshTokens;

    public LoginHandler(IUserRepository users, IPasswordHasher hasher, ITokenService tokens, IRefreshTokenRepository refreshTokens)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _refreshTokens = refreshTokens;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var user = await _users.GetByEmailAsync(email);
        if (user == null || !_hasher.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var accessToken = _tokens.CreateAccessToken(user.Id, user.Email.Value, user.Role);
        var refresh = _tokens.CreateRefreshToken();
        await _refreshTokens.SaveAsync(user.Id, refresh.Token, refresh.ExpiresAt);

        return new AuthResponse(accessToken, refresh.Token, refresh.ExpiresAt, new UserDto(user.Id, user.Email.Value, user.Role));
    }
}

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly ITokenService _tokens;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;

    public RefreshTokenHandler(ITokenService tokens, IRefreshTokenRepository refreshTokens, IUserRepository users)
    {
        _tokens = tokens;
        _refreshTokens = refreshTokens;
        _users = users;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = JwtTokenReader.ReadPrincipal(request.AccessToken);
        var userId = Guid.Parse(principal.FindFirst("sub")!.Value);

        var valid = await _refreshTokens.ValidateAsync(userId, request.RefreshToken);
        if (!valid) throw new UnauthorizedAccessException("Invalid refresh token");

        var user = await _users.GetByIdAsync(userId);
        if (user == null) throw new UnauthorizedAccessException("User not found");

        await _refreshTokens.RevokeAsync(userId, request.RefreshToken);

        var accessToken = _tokens.CreateAccessToken(user.Id, user.Email.Value, user.Role);
        var refresh = _tokens.CreateRefreshToken();
        await _refreshTokens.SaveAsync(user.Id, refresh.Token, refresh.ExpiresAt);

        return new AuthResponse(accessToken, refresh.Token, refresh.ExpiresAt, new UserDto(user.Id, user.Email.Value, user.Role));
    }
}

public sealed class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand, UserDto>
{
    private readonly IUserRepository _users;

    public ChangeUserRoleHandler(IUserRepository users) => _users = users;

    public async Task<UserDto> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var role = request.Role;
        if (!string.Equals(role, Roles.User, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid role");

        var normalizedRole = string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase)
            ? Roles.Admin
            : Roles.User;

        var updated = await _users.UpdateRoleAsync(request.UserId, normalizedRole);
        if (!updated) throw new InvalidOperationException("User not found");

        await _users.SaveChangesAsync();

        var user = await _users.GetByIdAsync(request.UserId);
        if (user == null) throw new InvalidOperationException("User not found");

        return new UserDto(user.Id, user.Email.Value, user.Role);
    }
}

public sealed class BootstrapAdminHandler : IRequestHandler<BootstrapAdminCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IIntegrationEventPublisher _publisher;
    private readonly IConfiguration _config;

    public BootstrapAdminHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokens,
        IRefreshTokenRepository refreshTokens,
        IIntegrationEventPublisher publisher,
        IConfiguration config)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _refreshTokens = refreshTokens;
        _publisher = publisher;
        _config = config;
    }

    public async Task<AuthResponse> Handle(BootstrapAdminCommand request, CancellationToken cancellationToken)
    {
        var secret = _config["AdminBootstrap:Secret"];
        if (string.IsNullOrWhiteSpace(secret) || !string.Equals(secret, request.Secret, StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid bootstrap secret");

        var hasAdmin = await _users.AnyAdminAsync();
        if (hasAdmin) throw new InvalidOperationException("Admin already exists");

        var email = new Email(request.Email);
        var existing = await _users.GetByEmailAsync(email);
        if (existing != null) throw new InvalidOperationException("Email already registered");

        var id = Guid.NewGuid();
        var hash = _hasher.Hash(request.Password);
        var user = User.Register(id, email, hash, Roles.Admin);

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        await _publisher.PublishAsync(new UserRegisteredEvent(user.Id, user.Email.Value, user.Role));

        var accessToken = _tokens.CreateAccessToken(user.Id, user.Email.Value, user.Role);
        var refresh = _tokens.CreateRefreshToken();
        await _refreshTokens.SaveAsync(user.Id, refresh.Token, refresh.ExpiresAt);

        return new AuthResponse(accessToken, refresh.Token, refresh.ExpiresAt, new UserDto(user.Id, user.Email.Value, user.Role));
    }
}

internal static class JwtTokenReader
{
    public static System.Security.Claims.ClaimsPrincipal ReadPrincipal(string token)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var identity = new System.Security.Claims.ClaimsIdentity(jwt.Claims, "jwt");
        return new System.Security.Claims.ClaimsPrincipal(identity);
    }
}
