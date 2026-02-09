using IdentityService.Domain;

namespace IdentityService.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> UpdateRoleAsync(Guid id, string role);
    Task<bool> AnyAdminAsync();
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
