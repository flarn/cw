using System.Text.Json;
using CW.Core.Events;
using CW.Core.interfaces;

namespace CW.Worker.Shared;

public class JsonSystem : IExternalSystem
{
    public string DirectoryPath => Path.Combine("..", "externalSystem", "json");

    public async Task SyncOrder(OrderUpdatedEvent orderUpdatedEvent, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        var filePath = Path.Combine(DirectoryPath, $"{orderUpdatedEvent.Id}.json");

        if (File.Exists(filePath))
        {
            var existingOrder = await JsonSerializer.DeserializeAsync<OrderUpdatedEvent>(File.OpenRead(filePath), Core.JsonSerializationSettings.Instance, cancellationToken) ??
                                throw new JsonException("Unable to deserialize order");

            if (existingOrder.UpdatedAt > orderUpdatedEvent.UpdatedAt)
                throw new Exception("Order is modified later than event");

            File.Delete(filePath);
        }

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, orderUpdatedEvent, Core.JsonSerializationSettings.Instance, cancellationToken);
    }
}