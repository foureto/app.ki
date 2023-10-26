using AppKi.Client.DataAccess;
using AppKi.Client.DataAccess.Domain;

namespace AppKi.Client.Jobs;

internal class DataHistoryStoringJob : BackgroundService
{
    private readonly AppChContext _chContext;
    private readonly IServiceProvider _serviceProvider;

    public DataHistoryStoringJob(AppChContext chContext, IServiceProvider serviceProvider)
    {
        _chContext = chContext;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (var i = 0; i < 10000; i++)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10), stoppingToken);
            await _chContext.Ohlcvs.Write(new Ohlcv {Stamp = DateTime.UtcNow});
        }
    }
}