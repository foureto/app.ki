using System.Collections.Concurrent;
using System.Threading.Channels;
using App.Ki.Business.Services.Exchanges.Models;
using App.Ki.Commons.Domain.Exchange;

namespace App.Ki.Business.Services.Feed;

public class DataYard : IDataYard
{
    private readonly ConcurrentDictionary<Symbol, PairInfo> _pairs = new();

    private readonly ConcurrentDictionary<Symbol, Ticker> _tickers = new();
    private readonly ConcurrentDictionary<Func<Ticker, bool>, Channel<Ticker>> _tickerChannels = new();

    public IEnumerable<Ticker> Tickers => _tickers.Values;

    public void Enqueue(IEnumerable<PairInfo> pairs)
    {
        foreach (var pair in pairs)
            if (!_pairs.TryAdd(pair.Symbol, pair))
                _pairs[pair.Symbol] = pair;
    }

    public void Enqueue(Ticker ticker)
    {
        if (!_tickers.TryAdd(ticker.Symbol, ticker))
            _tickers[ticker.Symbol] = ticker;

        foreach (var func in _tickerChannels.Keys)
            if (func(ticker))
                _tickerChannels[func].Writer.TryWrite(ticker);
    }

    public ChannelReader<Ticker> Subscribe(Func<Ticker, bool> filter)
    {
        if (_tickerChannels.TryGetValue(filter, out var channel))
            return channel.Reader;
        
        return _tickerChannels.TryAdd(filter, Channel.CreateUnbounded<Ticker>())
            ? _tickerChannels[filter].Reader
            : null;
    }

    public bool UnSubscribe(Func<Ticker, bool> filter)
    {
        if (!_tickerChannels.TryRemove(filter, out var channel))
            return false;

        channel.Writer.TryComplete();
        return true;
    }
}