using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure.Entities;

namespace IdentityService.Infrastructure;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserEntity>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<RefreshTokenEntity>().HasIndex(r => r.Token).IsUnique();
        modelBuilder.Entity<RefreshTokenEntity>().HasIndex(r => r.UserId);
    }
}
