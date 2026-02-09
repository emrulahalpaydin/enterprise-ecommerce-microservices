using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries;

public sealed record GetPaymentQuery(Guid Id) : IRequest<PaymentDto?>;
public sealed record ListPaymentsQuery() : IRequest<IReadOnlyCollection<PaymentDto>>;
