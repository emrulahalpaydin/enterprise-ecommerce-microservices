using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Infrastructure.Entities;

public class OrderItemEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    [Column(TypeName = "numeric(18,2)")]
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public OrderEntity? Order { get; set; }
}
