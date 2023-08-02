using System.Collections.Concurrent;
using System.Threading.Channels;
using App.Ki.Business.Hubs;
using App.Ki.Commons.Domain.Exchange;
using Microsoft.AspNetCore.SignalR;

namespace App.Ki.Business.Services.Feed;

public interface IFeedPublisher
{
    void PublishTickers(string group, ChannelReader<Ticker> reader);
}

internal class FeedPublisher : IFeedPublisher
{
    private readonly IHubContext<FeedHub, IFeedHub> _hubContext;
    private readonly ConcurrentDictionary<string, Task> _runningGroups = new();

    public FeedPublisher(IHubContext<FeedHub, IFeedHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void PublishTickers(string group, ChannelReader<Ticker> reader)
    {
        if (_runningGroups.ContainsKey(group))
            return;

        var task = Task.Run(async () =>
        {
            await foreach (var ticker in reader.ReadAllAsync())
                await _hubContext.Clients.Group(group).Ticker(ticker);
        });

        _runningGroups.TryAdd(group, task);
    }
}