using IdentityService.Application.Interfaces;
using IdentityService.Domain;
using IdentityService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;
    public UserRepository(IdentityDbContext db) => _db = db;
    public Task AddAsync(User user)
    {
        _db.Users.Add(new UserEntity
        {
            Id = user.Id,
            Email = user.Email.Value,
            PasswordHash = user.PasswordHash,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        });
        return Task.CompletedTask;
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        var ent = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email.Value);
        return ent == null ? null : User.Load(ent.Id, new Email(ent.Email), ent.PasswordHash, ent.Role, ent.CreatedAt);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var ent = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return ent == null ? null : User.Load(ent.Id, new Email(ent.Email), ent.PasswordHash, ent.Role, ent.CreatedAt);
    }

    public async Task<bool> UpdateRoleAsync(Guid id, string role)
    {
        var ent = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (ent == null) return false;
        ent.Role = role;
        return true;
    }

    public Task<bool> AnyAdminAsync()
        => _db.Users.AsNoTracking().AnyAsync(u => u.Role == Roles.Admin);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
