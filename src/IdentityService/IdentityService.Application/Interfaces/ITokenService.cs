namespace IdentityService.Application.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(Guid userId, string email, string role);
    (string Token, DateTime ExpiresAt) CreateRefreshToken();
}
