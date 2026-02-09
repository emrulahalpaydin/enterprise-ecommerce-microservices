using Microservices.Shared.BuildingBlocks.Application;
using Microservices.Shared.EventBus;

namespace OrderService.Infrastructure;

public sealed class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IEventBus _bus;
    public IntegrationEventPublisher(IEventBus bus) => _bus = bus;
    public Task PublishAsync<T>(T @event) where T : notnull => _bus.PublishAsync(@event);
}
