using System.Runtime.CompilerServices;
using App.Ki.Business.Services.Exchanges.Models;
using App.Ki.Commons.Domain.Exchange;
using App.Ki.Commons.Models;
using Google.Protobuf.Collections;
using Grpc.Core;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace App.Ki.Business.Services.Exchanges.Internals;

internal class TinkoffClient : IExchange
{
    private readonly InvestApiClient _apiClient;

    public TinkoffClient(InvestApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<AppResultList<PairInfo>> GetPairs(CancellationToken token = default)
    {
        var data = await _apiClient.Instruments.SharesAsync(token);
        var result = data.Instruments.Select(e => new PairInfo
        {
            Symbol = new Symbol(e.Ticker, e.Currency, e.Figi, "Tinkoff"),
            BaseIncrement = e.MinPriceIncrement.ToDecimal(),
            BaseMinSize = 1,
            PriceIncrement = e.Nominal.ToDecimal(),
        });

        return AppResultList<PairInfo>.Ok(result);
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

    public async IAsyncEnumerable<Ticker> SubscribeTickers([EnumeratorCancellation] CancellationToken token = default)
    {
        var shares = (await _apiClient.Instruments.GetAssetsAsync(
                new AssetsRequest { InstrumentType = InstrumentType.Share }))
            .Assets.SelectMany(e => e.Instruments).ToDictionary(e => e.Figi, e => e);

        using var stream = _apiClient.MarketDataStream.MarketDataStream();
        await stream.RequestStream.WriteAsync(new MarketDataRequest
        {
            SubscribeLastPriceRequest = new SubscribeLastPriceRequest
            {
                Instruments = { shares.Keys.Select(k => new LastPriceInstrument { Figi = k }) },
                SubscriptionAction = SubscriptionAction.Subscribe,
            },
        }, token);

        await foreach (var pack in stream.ResponseStream.ReadAllAsync(cancellationToken: token))
        {
            if (pack.LastPrice is null)
                continue;

            var price = pack.LastPrice.Price.ToDecimal();

            yield return new Ticker(
                new Symbol(shares[pack.LastPrice.Figi].Ticker, "RUB", pack.LastPrice.Figi, "Tinkoff"),
                price, price, price,
                DateTime.UtcNow);
        }
    }
}