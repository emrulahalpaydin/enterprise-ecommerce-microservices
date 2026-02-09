using System.ComponentModel.DataAnnotations;

namespace OrderService.Infrastructure.Entities;

public class BasketEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<BasketItemEntity> Items { get; set; } = new List<BasketItemEntity>();
}
