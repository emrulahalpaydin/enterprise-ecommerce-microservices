using Microservices.Shared.BuildingBlocks.Application;
using Microservices.Shared.Contracts;
using PaymentService.Application;
using PaymentService.Domain;

namespace PaymentService.Infrastructure;

public class PaymentProcessor : IPaymentProcessor
{
    private readonly IIntegrationEventPublisher _publisher;
    private readonly IPaymentRepository _repo;
    public PaymentProcessor(IIntegrationEventPublisher publisher, IPaymentRepository repo) { _publisher = publisher; _repo = repo; }

    public async Task HandleOrderCreatedAsync(OrderCreatedEvent e)
    {
        Console.WriteLine($"PaymentService processing order {e.OrderId}");
        await Task.Delay(500);
        var success = (new Random()).Next(0, 10) > 1;
        var payment = Payment.Create(Guid.NewGuid(), e.OrderId, e.Total, "card");
        if (success) payment.MarkCompleted(Guid.NewGuid().ToString("N"));
        else payment.MarkFailed("Insufficient funds");
        await _repo.AddAsync(payment);
        await _repo.SaveChangesAsync();

        if (success)
        {
            await _publisher.PublishAsync(new PaymentCompletedEvent(payment.Id, e.OrderId, e.Total));
            Console.WriteLine($"Payment completed for order {e.OrderId}");
        }
        else
        {
            await _publisher.PublishAsync(new PaymentFailedEvent(payment.Id, e.OrderId, e.Total, "Insufficient funds"));
            Console.WriteLine($"Payment failed for order {e.OrderId}");
        }
    }
}
