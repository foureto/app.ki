using System.Threading.Channels;
using App.Ki.Business.Services.Exchanges.Models;
using App.Ki.Commons.Domain.Exchange;

namespace App.Ki.Business.Services.Feed;

public interface IDataYard
{
    IEnumerable<Ticker> Tickers { get; }

    void Enqueue(IEnumerable<PairInfo> pairs);
    void Enqueue(Ticker ticker);
    ChannelReader<Ticker> Subscribe(Func<Ticker, bool> filter);
    bool UnSubscribe(Func<Ticker, bool> filter);
}