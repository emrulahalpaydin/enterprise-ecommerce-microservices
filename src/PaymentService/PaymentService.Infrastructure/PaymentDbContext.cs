using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure.Entities;

namespace PaymentService.Infrastructure;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> opts) : base(opts) { }
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PaymentEntity>().Property(p => p.Amount).HasPrecision(18,2);
    }
}
