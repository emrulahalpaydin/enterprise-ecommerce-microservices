using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Entities;

namespace OrderService.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> opts) : base(opts) { }
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<BasketEntity> Baskets => Set<BasketEntity>();
    public DbSet<BasketItemEntity> BasketItems => Set<BasketItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OrderEntity>().Property(o => o.Total).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItemEntity>().Property(i => i.UnitPrice).HasPrecision(18,2);
        modelBuilder.Entity<OrderItemEntity>().HasOne(i => i.Order).WithMany(o => o.Items).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BasketItemEntity>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<BasketItemEntity>().HasOne(i => i.Basket).WithMany(b => b.Items).HasForeignKey(i => i.BasketId).OnDelete(DeleteBehavior.Cascade);
    }
}
