using CW.Core.Events;
using CW.Core.interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace CW.Worker.Shared.Tests;

public class WorkerTests
{
    [Test]
    public async Task WorkerCallsProcessOnce()
    {
        var bus = Substitute.For<IBusConsumer>();

        var externalSystem = Substitute.For<IExternalSystem>();
        var timeProvider = new FakeTimeProvider();

        var worker = new Worker(bus, externalSystem, timeProvider, NullLogger<Worker>.Instance);
        await worker.StartAsync(CancellationToken.None);

        timeProvider.Advance(TimeSpan.FromSeconds(31));
        await worker.StopAsync(CancellationToken.None);

        await bus.Received(1).ProcessMessages<OrderUpdatedEvent>(Arg.Any<Func<OrderUpdatedEvent, Task>>());
    }

    [Test]
    public async Task WorkerCallsProcessTwice()
    {
        var bus = Substitute.For<IBusConsumer>();

        var externalSystem = Substitute.For<IExternalSystem>();
        var timeProvider = new FakeTimeProvider();

        var worker = new Worker(bus, externalSystem, timeProvider, NullLogger<Worker>.Instance);
        await worker.StartAsync(CancellationToken.None);

        timeProvider.Advance(TimeSpan.FromSeconds(31));
        timeProvider.Advance(TimeSpan.FromSeconds(31));
        await worker.StopAsync(CancellationToken.None);

        await bus.Received(2).ProcessMessages<OrderUpdatedEvent>(Arg.Any<Func<OrderUpdatedEvent, Task>>());
    }
}