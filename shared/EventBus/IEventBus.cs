using System;
using System.Threading.Tasks;

namespace Microservices.Shared.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event) where T : notnull;
        void Subscribe<T>(Func<T, Task> handler) where T : notnull;
    }
}
