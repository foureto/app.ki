using App.Ki.Business.Hubs;
using App.Ki.Business.Services.Exchanges;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace App.Ki.Business.Jobs;

[DisallowConcurrentExecution]
internal class TickerJob : IJob
{
    private readonly IExchangeFactory _factory;
    private readonly IHubContext<FeedHub, IFeedHub> _feedHub;
    private readonly ILogger<TickerJob> _logger;

    public TickerJob(IExchangeFactory factory, IHubContext<FeedHub, IFeedHub> feedHub, ILogger<TickerJob> logger)
    {
        _factory = factory;
        _feedHub = feedHub;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var exchanges = _factory.GetAll();
        foreach (var exchange in exchanges)
        {
            var tickers = await exchange.GetTickers(context.CancellationToken);
            if (!tickers.Success) continue;

            await _feedHub.Clients.All.Tickers(tickers.Data);

            _logger.LogInformation("Got tickers from {Exchange}", exchange.GetType().Name);
        }

        _logger.LogInformation("Get tickers stub");
    }
}