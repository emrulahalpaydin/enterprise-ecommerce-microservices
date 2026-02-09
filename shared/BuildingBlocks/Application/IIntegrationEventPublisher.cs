namespace Microservices.Shared.BuildingBlocks.Application;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<T>(T @event) where T : notnull;
}
