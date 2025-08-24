using Azure.Messaging.ServiceBus;
using CW.Core;
using CW.Core.interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CW.Infra.ServiceBus;

public class ServicebusConsumer : IBusConsumer, IAsyncDisposable
{
    private readonly ServiceBusReceiver _reader;
    private readonly ILogger<ServicebusConsumer> _logger;

    public ServicebusConsumer(ServiceBusClient serviceBusClient, IOptions<SubscriberConfiguration> options, ILogger<ServicebusConsumer> logger)
    {
        _reader = serviceBusClient.CreateReceiver(options.Value.Topic, options.Value.Subscriber);
        _logger = logger;
    }

    public async Task ProcessMessages<TMessage>(Func<TMessage, Task> callback) where TMessage : IEvent
    {
        var messages = await _reader.ReceiveMessagesAsync(100, TimeSpan.FromSeconds(10));

        foreach (var message in messages)
        {
            try
            {
                var dto = message.Body.ToObjectFromJson<TMessage>(JsonSerializationSettings.Instance) ?? throw new JsonException("Message body is null or invalid");

                await callback(dto);

                await _reader.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(ex, "Error processing message with ID {MessageId}", message.MessageId);
            }
        }
    }
    public async ValueTask DisposeAsync()
    {
        await _reader.DisposeAsync();
    }
}
