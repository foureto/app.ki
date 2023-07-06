using Microsoft.Extensions.Logging;
using Quartz;

namespace App.Ki.Business.Jobs;

[DisallowConcurrentExecution]
internal class TickerJob : IJob
{
    private readonly ILogger<TickerJob> _logger;

    public TickerJob(ILogger<TickerJob> logger)
    {
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Yield();
        _logger.LogInformation("Get tickers stub");
    }
}