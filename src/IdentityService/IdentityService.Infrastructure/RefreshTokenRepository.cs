using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _db;
    public RefreshTokenRepository(IdentityDbContext db) => _db = db;

    public async Task SaveAsync(Guid userId, string token, DateTime expiresAt)
    {
        _db.RefreshTokens.Add(new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            Revoked = false
        });
        await _db.SaveChangesAsync();
    }

    public async Task<bool> ValidateAsync(Guid userId, string token)
    {
        var rt = await _db.RefreshTokens.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token && !x.Revoked);
        return rt != null && rt.ExpiresAt > DateTime.UtcNow;
    }

    public async Task RevokeAsync(Guid userId, string token)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token);
        if (rt == null) return;
        rt.Revoked = true;
        await _db.SaveChangesAsync();
    }
}
