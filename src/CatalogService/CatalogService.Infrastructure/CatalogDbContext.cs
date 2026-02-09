using Microsoft.EntityFrameworkCore;
using CatalogService.Infrastructure.Entities;

namespace CatalogService.Infrastructure;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> opts) : base(opts) { }
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<ProductVariantEntity> ProductVariants => Set<ProductVariantEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProductEntity>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ProductEntity>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<ProductVariantEntity>().HasOne(v => v.Product).WithMany(p => p.Variants).HasForeignKey(v => v.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}
