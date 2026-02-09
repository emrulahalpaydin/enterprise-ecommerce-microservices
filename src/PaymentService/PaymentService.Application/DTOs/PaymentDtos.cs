namespace PaymentService.Application.DTOs;

public sealed record PaymentDto(Guid Id, Guid OrderId, decimal Amount, string Status, string? Method, string? TransactionId, DateTime CreatedAt);
