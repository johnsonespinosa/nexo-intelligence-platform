using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Infrastructure.Services;

public class InMemoryEventBusService : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();
    private readonly ILogger<InMemoryEventBusService>? _logger;

    public InMemoryEventBusService(ILogger<InMemoryEventBusService>? logger = null)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        var eventType = typeof(T);
        _logger?.LogDebug("Publishing event {EventType}", eventType.Name);

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                _ = Task.Run(() => handler(@event), cancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        var eventType = typeof(T);

        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Func<object, Task>>();
        }

        _handlers[eventType].Add(async (e) => await handler((T)e));

        _logger?.LogDebug("Subscribed to event {EventType}", eventType.Name);

        return Task.CompletedTask;
    }
}