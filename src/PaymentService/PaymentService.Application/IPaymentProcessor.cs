using Microservices.Shared.Contracts;

namespace PaymentService.Application;

public interface IPaymentProcessor
{
    Task HandleOrderCreatedAsync(OrderCreatedEvent e);
}
