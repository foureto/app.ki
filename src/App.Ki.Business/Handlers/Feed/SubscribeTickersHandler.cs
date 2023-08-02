using App.Ki.Business.Hubs;
using App.Ki.Business.Models.Filters;
using App.Ki.Business.Services.Feed;
using App.Ki.Commons.Models;
using Mediator;
using Microsoft.AspNetCore.SignalR;

namespace App.Ki.Business.Handlers.Feed;

public class SubscribeTickersQuery : IRequest<AppResult>
{
    public TickerFilterDto Filter { get; }
    public string Connection { get; }

    public SubscribeTickersQuery(TickerFilterDto filter, string connection)
    {
        Filter = filter;
        Connection = connection;
    }
}

public class SubscribeTickersHandler : IRequestHandler<SubscribeTickersQuery, AppResult>
{
    private readonly IDataYard _yard;
    private readonly IFeedPublisher _publisher;
    private readonly IHubContext<FeedHub, IFeedHub> _hubContext;

    public SubscribeTickersHandler(
        IDataYard yard,
        IFeedPublisher publisher,
        IHubContext<FeedHub, IFeedHub> hubContext)
    {
        _yard = yard;
        _publisher = publisher;
        _hubContext = hubContext;
    }

    public async ValueTask<AppResult> Handle(SubscribeTickersQuery request, CancellationToken cancellationToken)
    {
        var key = request.Filter.ToKey();
        var filter = request.Filter.ToFunc();
        var reader = _yard.Subscribe(filter);
        if (reader == null)
            return AppResult.Failed("Could not subscribe");

        await _hubContext.Groups.AddToGroupAsync(request.Connection, key, cancellationToken);
        _publisher.PublishTickers(key, reader);

        return AppResult.Ok();
    }
}