namespace IdentityService.Application.DTOs;

public sealed record UserDto(Guid Id, string Email, string Role);

public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiresAt, UserDto User);
