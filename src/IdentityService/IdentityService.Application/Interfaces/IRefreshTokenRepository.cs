namespace IdentityService.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task SaveAsync(Guid userId, string token, DateTime expiresAt);
    Task<bool> ValidateAsync(Guid userId, string token);
    Task RevokeAsync(Guid userId, string token);
}
