using App.Ki.Clickhouse.Helpers;
using App.Ki.Clickhouse.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Ki.Clickhouse.Internals;

internal class BufferFlusher : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IOptions<ClickhouseSettings> _options;
    private readonly ILogger<BufferFlusher> _logger;

    public BufferFlusher(
        IServiceProvider provider,
        IOptions<ClickhouseSettings> options,
        ILogger<BufferFlusher> logger)
    {
        _provider = provider;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWork(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Flushing buffer iteration failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.Value.Buffer.FlushInSeconds), stoppingToken);
        }
    }

    private async Task DoWork(CancellationToken token)
    {
        await using var scope = _provider.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<ClickhouseConnectionFactory>();

        foreach (var type in QueuedBuffer.Types)
        {
            var items = QueuedBuffer.Pop(
                type, _options.Value.Buffer.MaxGetFromBuffer,
                _options.Value.Buffer.MinGetFromBuffer).ToList();

            if (items.Count == 0)
                continue;

            try
            {
                await (await factory.GetConnection()).BulkAdd(type, items, token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not flush data");
                items.ForEach(QueuedBuffer.Push);
                throw;
            }
        }
    }
}