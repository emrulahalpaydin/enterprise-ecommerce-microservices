using System.ComponentModel.DataAnnotations;

namespace CatalogService.Infrastructure.Entities;

public class CategoryEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}
