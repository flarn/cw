using Azure.Messaging.ServiceBus;
using CW.Core;
using CW.Core.interfaces;
using System.Collections.Concurrent;

namespace CW.Infra.ServiceBus;

public class ServiceBusSender(ServiceBusClient serviceBusClient) : IBusSender, IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, Azure.Messaging.ServiceBus.ServiceBusSender> _senders = new();

    public Task SendMessage<TMessage>(string topic, TMessage message, CancellationToken cancellationToken) where TMessage : IEvent
    {
        return _senders.GetOrAdd(topic, (key, client) => client.CreateSender(topic), serviceBusClient)
            .SendMessageAsync(new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(message, JsonSerializationSettings.Instance)), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var sender in _senders.Values)
        {
            await sender.DisposeAsync();
        }
    }
}