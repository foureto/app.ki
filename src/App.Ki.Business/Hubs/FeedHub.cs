using App.Ki.Business.Services.Identity;
using App.Ki.Commons.Domain.Exchange;
using Microsoft.AspNetCore.SignalR;

namespace App.Ki.Business.Hubs;

public interface IFeedHub
{
    Task Tickers(IEnumerable<Ticker> tickers, CancellationToken token = default);
}

public class FeedHub : Hub<IFeedHub>
{
    private readonly ICurrentUserService<int> _userService;
    private readonly IHubContext<FeedHub, IFeedHub> _hubContext;

    public FeedHub(
        ICurrentUserService<int> userService,
        IHubContext<FeedHub, IFeedHub> hubContext)
    {
        _userService = userService;
        _hubContext = hubContext;
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