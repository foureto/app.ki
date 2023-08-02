using App.Ki.Business.Handlers.Feed;
using App.Ki.Business.Models.Filters;
using App.Ki.Business.Services.Identity;
using App.Ki.Commons.Domain.Exchange;
using Mediator;
using Microsoft.AspNetCore.SignalR;

namespace App.Ki.Business.Hubs;

public interface IFeedHub
{
    Task Tickers(IEnumerable<Ticker> tickers, CancellationToken token = default);
    Task Ticker(Ticker ticker, CancellationToken token = default);
}

public class FeedHub : Hub<IFeedHub>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService<int> _userService;

    public FeedHub(IMediator mediator, ICurrentUserService<int> userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    public async Task SubscribeTickers(TickerFilterDto filter)
    {
        await _mediator.Send(new SubscribeTickersQuery(filter ?? new(), Context.ConnectionId));
    }

    public override Task OnConnectedAsync()
    {
        var userId = _userService.GetCurrentUser().Id;
        ConnectionsStore.AddConnection(userId, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        ConnectionsStore.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}