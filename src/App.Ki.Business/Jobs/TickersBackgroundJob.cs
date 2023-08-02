using App.Ki.Business.Hubs;
using App.Ki.Business.Services.Exchanges;
using App.Ki.Business.Services.Feed;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Ki.Business.Jobs;

internal class TickersBackgroundJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDataYard _dataYard;
    private readonly IHubContext<FeedHub, IFeedHub> _feedHub;

    public TickersBackgroundJob(
        IServiceProvider serviceProvider,
        IDataYard dataYard,
        IHubContext<FeedHub, IFeedHub> feedHub)
    {
        _serviceProvider = serviceProvider;
        _dataYard = dataYard;
        _feedHub = feedHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<IExchangeFactory>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var source = new CancellationTokenSource();
            var tasks = factory.GetAll().Select(e => RunSubscription(e, source.Token)).ToArray();

            try
            {
                await Task.WhenAll(tasks);
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            catch (Exception e)
            {
                source.Cancel();
            }
        }
    }

    private async Task RunSubscription(IExchange exchange, CancellationToken token)
    {
        await foreach (var item in exchange.SubscribeTickers(token))
            _dataYard.Enqueue(item);
    }
}