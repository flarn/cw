using CW.Core.Events;
using CW.Core.interfaces;

namespace cw.worker.j;

public class Worker : BackgroundService
{
    private readonly IBusConsumer _busConsumer;
    private readonly ILogger<Worker> _logger;
    private readonly PeriodicTimer _timer;

    public Worker(IBusConsumer busConsumer, ILogger<Worker> logger)
    {
        _busConsumer = busConsumer;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Started Worker {workerName}", this.GetType().FullName);

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Started process messages");

            await _busConsumer.ProcessMessages<OrderUpdatedEvent>(async (message) =>
            {
                _logger.LogInformation(message.ToString());
            });

            _logger.LogInformation("Completed process messages");
        }

        _logger.LogInformation("Stopped Worker {workerName}", this.GetType().FullName);
    }
}
