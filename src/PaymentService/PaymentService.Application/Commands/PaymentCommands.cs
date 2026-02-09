using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Commands;

public sealed record CreatePaymentCommand(Guid OrderId, decimal Amount, string? Method) : IRequest<PaymentDto>;
public sealed record MarkPaymentCompletedCommand(Guid PaymentId, string TransactionId) : IRequest<PaymentDto>;
public sealed record MarkPaymentFailedCommand(Guid PaymentId, string Reason) : IRequest<PaymentDto>;
