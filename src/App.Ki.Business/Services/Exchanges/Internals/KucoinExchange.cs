using App.Ki.Business.Services.Exchanges.Models;
using App.Ki.Business.Services.Exchanges.Settings;
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

    public Task<AppResultList<PairInfo>> GetPairs(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<AppResultList<TickerInfo>> GetTickers(CancellationToken token = default)
    {
        var callResult = await _client.SpotApi.ExchangeData.GetTickersAsync(token);
        var result = callResult.Success
            ? callResult.Data.Data.Select(e => new TickerInfo
            {
                Exchange = "Kucoin",
                Bid = e.BestBidPrice.GetValueOrDefault(),
                Ask = e.BestAskPrice.GetValueOrDefault(),
                BaseVolume = e.Volume.GetValueOrDefault(),
                QuotedVolume = e.QuoteVolume.GetValueOrDefault(),
                ApiSymbol = e.Symbol
            })
            : new List<TickerInfo>();

        return AppResultList<TickerInfo>.Ok(result);
    }

    public Task<AppResult<OrderBookInfo>> GetOrderBook(
        string apiSymbol, int depth = 100, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}