using CW.Core.Events;

namespace CW.Worker.Shared.Tests;

public class JsonSystemTests
{
    [Test]
    public async Task OrderShouldGetCreated()
    {
        var jsonSystem = new JsonSystem();
        var orderUpdatedEvent = new OrderUpdatedEvent(Guid.NewGuid(), "test", 1, 100, DateTime.Now);
        var filePath = Path.Combine(jsonSystem.DirectoryPath, $"{orderUpdatedEvent.Id}.json");

        try
        {
            await jsonSystem.SyncOrder(orderUpdatedEvent, CancellationToken.None);
            var exists = File.Exists(filePath);
            Assert.That(exists, Is.True);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Test]
    public async Task OrderShouldThrowOld()
    {
        var jsonSystem = new JsonSystem();
        var orderUpdatedEvent = new OrderUpdatedEvent(Guid.NewGuid(), "test", 1, 100, DateTime.Now);
        var filePath = Path.Combine(jsonSystem.DirectoryPath, $"{orderUpdatedEvent.Id}.json");

        try
        {
            await jsonSystem.SyncOrder(orderUpdatedEvent, CancellationToken.None);

            Assert.ThrowsAsync<Exception>(async () => await jsonSystem.SyncOrder(orderUpdatedEvent with { UpdatedAt = orderUpdatedEvent.UpdatedAt.AddSeconds(-1) }, CancellationToken.None));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Test]
    public async Task OrderShouldReplaceNew()
    {
        var jsonSystem = new JsonSystem();
        var orderUpdatedEvent = new OrderUpdatedEvent(Guid.NewGuid(), "test", 1, 100, DateTime.Now);
        var updatedTime = orderUpdatedEvent.UpdatedAt.AddMinutes(1);
        var filePath = Path.Combine(jsonSystem.DirectoryPath, $"{orderUpdatedEvent.Id}.json");

        try
        {
            await jsonSystem.SyncOrder(orderUpdatedEvent, CancellationToken.None);

            await jsonSystem.SyncOrder(orderUpdatedEvent with { UpdatedAt = updatedTime }, CancellationToken.None);
            var file = System.Text.Json.JsonSerializer.Deserialize<OrderUpdatedEvent>(await File.ReadAllTextAsync(filePath), Core.JsonSerializationSettings.Instance)
                       ?? throw new Exception();

            Assert.That(file.UpdatedAt, Is.EqualTo(updatedTime));
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}