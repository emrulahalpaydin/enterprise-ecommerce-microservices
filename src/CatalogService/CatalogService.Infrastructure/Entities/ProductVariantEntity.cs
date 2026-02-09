using System.ComponentModel.DataAnnotations;

namespace CatalogService.Infrastructure.Entities;

public class ProductVariantEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public ProductEntity? Product { get; set; }
}
