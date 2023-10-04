using App.Ki.Business.Services.Exchanges.Helpers;
using App.Ki.Business.Services.Exchanges.Models;
using App.Ki.Commons.Domain.Exchange;
using App.Ki.Commons.Models;
using Microsoft.Extensions.Logging;

namespace App.Ki.Business.Services.Exchanges.Internals;

[Exchange("BingX")]
internal class BingXchange : BasicHttpClient, IExchange
{
    protected BingXchange(HttpClient client, ILogger<BingXchange> logger) : base(client, logger)
    {
    }

    public Task<AppResultList<PairInfo>> GetPairs(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<AppResultList<Ticker>> GetTickers(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<AppResult<OrderBookInfo>> GetOrderBook(
        string apiSymbol, int depth = 100, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Ticker> SubscribeTickers(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}