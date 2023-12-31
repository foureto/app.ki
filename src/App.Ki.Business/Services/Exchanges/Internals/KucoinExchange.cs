﻿using System.Runtime.CompilerServices;
using System.Threading.Channels;
using App.Ki.Business.Services.Exchanges.Models;
using App.Ki.Business.Services.Exchanges.Settings;
using App.Ki.Commons.Domain.Exchange;
using App.Ki.Commons.Models;
using Kucoin.Net.Clients;
using Kucoin.Net.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Ki.Business.Services.Exchanges.Internals;

[Exchange("Kucoin")]
internal class KucoinExchange : IExchange, IDisposable
{
    private readonly KucoinRestClient _client;
    private readonly IOptions<KucoinSettings> _options;
    private readonly ILogger<KucoinExchange> _logger;

    private const string Name = "Kucoin";

    public KucoinExchange(
        IHttpClientFactory factory,
        ILoggerFactory loggerFactory,
        IOptions<KucoinSettings> options,
        ILogger<KucoinExchange> logger)
    {
        _options = options;
        _logger = logger;

        _client = new KucoinRestClient(
            factory.CreateClient(nameof(KucoinExchange)),
            loggerFactory,
            opts =>
            {
                opts.ApiCredentials = new KucoinApiCredentials(
                    options.Value.ApiKey,
                    options.Value.ApiSecret,
                    options.Value.ApiSecretPhrase);
            });
    }

    public async Task<AppResultList<PairInfo>> GetPairs(CancellationToken token = default)
    {
        var callResult = await _client.SpotApi.ExchangeData.GetSymbolsAsync(ct: token);
        var result = callResult.Success
            ? callResult.Data.Select(e => new PairInfo
            {
                Symbol = new Symbol(e.BaseAsset, e.QuoteAsset, e.Symbol, "Kucoin"),
                BaseMinSize = e.BaseMinQuantity,
                BaseMaxSize = e.BaseMaxQuantity,
                BaseIncrement = e.BaseIncrement,
                PriceIncrement = e.PriceIncrement,
                QuotedMinSize = e.QuoteMinQuantity,
                QuotedMaxSize = e.QuoteMaxQuantity,
                QuotedIncrement = e.QuoteIncrement,
            })
            : new List<PairInfo>();
        return AppResultList<PairInfo>.Ok(result);
    }

    public async Task<AppResultList<Ticker>> GetTickers(CancellationToken token = default)
    {
        var callResult = await _client.SpotApi.ExchangeData.GetTickersAsync(token);
        var result = callResult.Success
            ? callResult.Data.Data.Select(e =>
            {
                var ticker = e.Symbol.Split('-');
                return new Ticker(
                    new Symbol(ticker[0], ticker[1], e.Symbol, Name),
                    e.BestBidPrice.GetValueOrDefault(),
                    e.BestAskPrice.GetValueOrDefault(),
                    e.AveragePrice.GetValueOrDefault(),
                    DateTime.UtcNow);
            })
            : new List<Ticker>();

        return AppResultList<Ticker>.Ok(result);
    }

    public async Task<AppResult<OrderBookInfo>> GetOrderBook(
        string apiSymbol, int depth = 100, CancellationToken token = default)
    {
        var callResult = await _client.SpotApi.ExchangeData.GetAggregatedPartialOrderBookAsync(apiSymbol, depth, token);
        if (!callResult.Success)
            return AppResult<OrderBookInfo>.Failed("Could not get order book");

        var result = new OrderBookInfo
        {
            Exchange = Name,
            ApiSymbol = callResult.Data.Symbol ?? apiSymbol,
            Asks = callResult.Data.Asks.Select(e =>
                new OrderBookEntry { Price = (double)e.Price, Quantity = (double)e.Quantity }).ToArray(),
            Bids = callResult.Data.Bids.Select(e =>
                new OrderBookEntry { Price = (double)e.Price, Quantity = (double)e.Quantity }).ToArray(),
        };

        return AppResult<OrderBookInfo>.Ok(result);
    }

    public async IAsyncEnumerable<Ticker> SubscribeTickers([EnumeratorCancellation] CancellationToken token = default)
    {
        using var socket = new KucoinSocketClient(opts =>
        {
            opts.ApiCredentials = new KucoinApiCredentials(
                _options.Value.ApiKey,
                _options.Value.ApiSecret,
                _options.Value.ApiSecretPhrase);
        });

        var channel = Channel.CreateUnbounded<Ticker>();
        var tickers = (await GetTickers(token)).Data.ToDictionary(e => e.Symbol.ApiSymbol, e => e);

        await socket.SpotApi.SubscribeToTickerUpdatesAsync(
            tickers.Keys.Take(10),
            e =>
            {
                var ticker = new Ticker(
                    tickers[e.Data.Symbol].Symbol,
                    (e.Data.BestBidPrice ?? e.Data.BestAskPrice ?? e.Data.LastPrice).GetValueOrDefault(),
                    (e.Data.BestAskPrice ?? e.Data.BestAskPrice ?? e.Data.LastPrice).GetValueOrDefault(),
                    (e.Data.LastPrice ?? e.Data.BestBidPrice ?? e.Data.BestAskPrice).GetValueOrDefault(),
                    DateTime.UtcNow);
                channel.Writer.TryWrite(ticker);
            },
            token);

        await foreach (var ticker in channel.Reader.ReadAllAsync(token))
            yield return ticker;

        channel.Writer.TryComplete();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}