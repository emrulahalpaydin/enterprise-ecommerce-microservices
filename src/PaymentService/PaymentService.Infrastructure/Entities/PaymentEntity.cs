using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Infrastructure.Entities;

public class PaymentEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    [Column(TypeName = "numeric(18,2)")]
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Method { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
