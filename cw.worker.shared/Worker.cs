using CW.Core.Events;
using CW.Core.interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CW.Worker.Shared;

public class Worker : BackgroundService
{
    private readonly IBusConsumer _busConsumer;
    private readonly IExternalSystem _externalSystem;
    private readonly ILogger<Worker> _logger;
    private readonly PeriodicTimer _timer;

    public Worker(IBusConsumer busConsumer, IExternalSystem externalSystem, TimeProvider timeProvider, ILogger<Worker> logger)
    {
        _busConsumer = busConsumer;
        _externalSystem = externalSystem;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(30), timeProvider);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Worker {workerName}", _externalSystem.GetType().FullName);

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Started process messages");

            await _busConsumer.ProcessMessages<OrderUpdatedEvent>(async (message) => { await _externalSystem.SyncOrder(message, stoppingToken); });

            _logger.LogInformation("Completed process messages");
        }

        _logger.LogInformation("Stopped Worker {workerName}", _externalSystem.GetType().FullName);
    }
}