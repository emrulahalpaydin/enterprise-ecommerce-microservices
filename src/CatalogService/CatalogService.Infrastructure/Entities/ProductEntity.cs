using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Infrastructure.Entities;

public class ProductEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    [Column(TypeName = "numeric(18,2)")]
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }
    public ICollection<ProductVariantEntity> Variants { get; set; } = new List<ProductVariantEntity>();
}
