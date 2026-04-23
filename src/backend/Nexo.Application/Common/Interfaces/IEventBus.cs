namespace Nexo.Application.Common.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class;
}
