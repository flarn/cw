
namespace CW.Core.interfaces;

public interface IBusConsumer
{
    Task ProcessMessages<TMessage>(Func<TMessage, Task> callback) where TMessage : IEvent;
}
