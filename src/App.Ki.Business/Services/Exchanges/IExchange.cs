using App.Ki.Business.Services.Exchanges.Models;
using App.Ki.Commons.Domain.Exchange;
using App.Ki.Commons.Models;

namespace App.Ki.Business.Services.Exchanges;

public interface IExchange
{
    Task<AppResultList<PairInfo>> GetPairs(CancellationToken token = default);
    Task<AppResultList<Ticker>> GetTickers(CancellationToken token = default);
    Task<AppResult<OrderBookInfo>> GetOrderBook(string apiSymbol, int depth = 100, CancellationToken token = default);
    IAsyncEnumerable<Ticker> SubscribeTickers(CancellationToken token = default);

}