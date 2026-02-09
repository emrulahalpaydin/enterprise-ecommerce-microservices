using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Infrastructure.Entities;

public class OrderEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [Column(TypeName = "numeric(18,2)")]
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<OrderItemEntity> Items { get; set; } = new List<OrderItemEntity>();
}
