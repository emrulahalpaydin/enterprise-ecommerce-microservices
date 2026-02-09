using MediatR;
using Microservices.Shared.BuildingBlocks.Application;
using Microservices.Shared.Contracts;
using PaymentService.Application.Commands;
using PaymentService.Application.DTOs;
using PaymentService.Application.Queries;
using PaymentService.Domain;

namespace PaymentService.Application.Handlers;

public sealed class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _payments;

    public CreatePaymentHandler(IPaymentRepository payments) => _payments = payments;

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.Create(Guid.NewGuid(), request.OrderId, request.Amount, request.Method);
        await _payments.AddAsync(payment);
        await _payments.SaveChangesAsync();
        return Map(payment);
    }

    private static PaymentDto Map(Payment p) => new(p.Id, p.OrderId, p.Amount, p.Status.ToString(), p.Method, p.TransactionId, p.CreatedAt);
}

public sealed class MarkPaymentCompletedHandler : IRequestHandler<MarkPaymentCompletedCommand, PaymentDto>
{
    private readonly IPaymentRepository _payments;
    private readonly IIntegrationEventPublisher _publisher;

    public MarkPaymentCompletedHandler(IPaymentRepository payments, IIntegrationEventPublisher publisher)
    {
        _payments = payments;
        _publisher = publisher;
    }

    public async Task<PaymentDto> Handle(MarkPaymentCompletedCommand request, CancellationToken cancellationToken)
    {
        var payment = await _payments.GetByIdAsync(request.PaymentId);
        if (payment == null) throw new InvalidOperationException("Payment not found");
        payment.MarkCompleted(request.TransactionId);
        await _payments.SaveChangesAsync();
        await _publisher.PublishAsync(new PaymentCompletedEvent(payment.Id, payment.OrderId, payment.Amount));
        return new PaymentDto(payment.Id, payment.OrderId, payment.Amount, payment.Status.ToString(), payment.Method, payment.TransactionId, payment.CreatedAt);
    }
}

public sealed class MarkPaymentFailedHandler : IRequestHandler<MarkPaymentFailedCommand, PaymentDto>
{
    private readonly IPaymentRepository _payments;
    private readonly IIntegrationEventPublisher _publisher;

    public MarkPaymentFailedHandler(IPaymentRepository payments, IIntegrationEventPublisher publisher)
    {
        _payments = payments;
        _publisher = publisher;
    }

    public async Task<PaymentDto> Handle(MarkPaymentFailedCommand request, CancellationToken cancellationToken)
    {
        var payment = await _payments.GetByIdAsync(request.PaymentId);
        if (payment == null) throw new InvalidOperationException("Payment not found");
        payment.MarkFailed(request.Reason);
        await _payments.SaveChangesAsync();
        await _publisher.PublishAsync(new PaymentFailedEvent(payment.Id, payment.OrderId, payment.Amount, request.Reason));
        return new PaymentDto(payment.Id, payment.OrderId, payment.Amount, payment.Status.ToString(), payment.Method, payment.TransactionId, payment.CreatedAt);
    }
}

public sealed class GetPaymentHandler : IRequestHandler<GetPaymentQuery, PaymentDto?>
{
    private readonly IPaymentRepository _payments;
    public GetPaymentHandler(IPaymentRepository payments) => _payments = payments;

    public async Task<PaymentDto?> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _payments.GetByIdAsync(request.Id);
        return payment == null ? null : new PaymentDto(payment.Id, payment.OrderId, payment.Amount, payment.Status.ToString(), payment.Method, payment.TransactionId, payment.CreatedAt);
    }
}

public sealed class ListPaymentsHandler : IRequestHandler<ListPaymentsQuery, IReadOnlyCollection<PaymentDto>>
{
    private readonly IPaymentRepository _payments;
    public ListPaymentsHandler(IPaymentRepository payments) => _payments = payments;

    public Task<IReadOnlyCollection<PaymentDto>> Handle(ListPaymentsQuery request, CancellationToken cancellationToken)
    {
        var data = _payments.Query().Select(p => new PaymentDto(p.Id, p.OrderId, p.Amount, p.Status.ToString(), p.Method, p.TransactionId, p.CreatedAt)).ToList();
        return Task.FromResult<IReadOnlyCollection<PaymentDto>>(data);
    }
}
