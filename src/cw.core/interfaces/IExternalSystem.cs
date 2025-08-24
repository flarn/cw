using CW.Core.Events;

namespace CW.Core.interfaces;

public interface IExternalSystem
{
    public Task SyncOrder(OrderUpdatedEvent orderUpdatedEvent, CancellationToken cancellationToken);
}
