using System.Text.Json;
using Nexo.Application.Common.Interfaces;
using StackExchange.Redis;

namespace Nexo.Infrastructure.Services;

public class EventBusService : IEventBus
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly string _streamName = "nexo-events";

    public EventBusService(IConnectionMultiplexer? redis)
    {
        _redis = redis;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        if (_redis == null || !_redis.IsConnected)
        {
            // Si no hay Redis, simplemente ignoramos (desarrollo)
            return;
        }

        try
        {
            var db = _redis.GetDatabase();
            var eventName = typeof(T).Name;
            var eventData = JsonSerializer.Serialize(@event);

            var entries = new NameValueEntry[]
            {
                new NameValueEntry(eventName, eventData)
            };

            await db.StreamAddAsync(
                _streamName,
                entries,
                maxLength: 10000,
                useApproximateMaxLength: true);
        }
        catch (Exception ex)
        {
            // Log error pero no fallar
            Console.WriteLine($"Redis publish error: {ex.Message}");
        }
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        if (_redis == null || !_redis.IsConnected)
        {
            // Sin Redis, no podemos suscribir
            return;
        }

        var db = _redis.GetDatabase();
        var eventName = typeof(T).Name;
        var lastId = "0-0";

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var entries = await db.StreamRangeAsync(_streamName, minId: lastId, count: 100);

                foreach (var entry in entries)
                {
                    foreach (var nameValue in entry.Values)
                    {
                        if (nameValue.Name.ToString() == eventName)
                        {
                            var eventObj = JsonSerializer.Deserialize<T>(nameValue.Value.ToString());
                            if (eventObj is not null)
                            {
                                await handler(eventObj);
                            }
                        }
                    }
                    lastId = entry.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis subscribe error: {ex.Message}");
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
}