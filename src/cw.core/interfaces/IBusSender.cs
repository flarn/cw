
namespace CW.Core.interfaces;

public interface IBusSender
{
    Task SendMessage<TMessage>(string topic, TMessage message, CancellationToken cancellationToken) where TMessage : IEvent;
}
