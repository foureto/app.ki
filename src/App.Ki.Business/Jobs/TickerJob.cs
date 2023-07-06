using App.Ki.Business.Services.Exchanges;
using Microsoft.Extensions.Logging;
using Quartz;

namespace App.Ki.Business.Jobs;

[DisallowConcurrentExecution]
internal class TickerJob : IJob
{
    private readonly IExchangeFactory _factory;
    private readonly ILogger<TickerJob> _logger;

    public TickerJob(IExchangeFactory factory, ILogger<TickerJob> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var exchanges = _factory.GetAll();
        foreach (var exchange in exchanges)
        {
            var result = await exchange.GetTickers(context.CancellationToken);
            if (!result.Success) continue;

            _logger.LogInformation("Got tickers from {Exchange}", exchange.GetType().Name);
        }

        _logger.LogInformation("Get tickers stub");
    }
}