using App.Ki.Business.Models.Filters;
using App.Ki.Business.Services.Feed;
using App.Ki.Commons.Domain.Exchange;
using App.Ki.Commons.Models;
using Mediator;

namespace App.Ki.Business.Handlers.Feed;

public class GetTickersQuery : IRequest<AppResultList<Ticker>>
{
    public TickerFilterDto Filter { get; }

    public GetTickersQuery(TickerFilterDto filter)
    {
        Filter = filter;
    }
}

public class GetTickersHandler : IRequestHandler<GetTickersQuery, AppResultList<Ticker>>
{
    private readonly IDataYard _yard;

    public GetTickersHandler(IDataYard yard)
    {
        _yard = yard;
    }

    public ValueTask<AppResultList<Ticker>> Handle(
        GetTickersQuery request, CancellationToken cancellationToken)
    {
        var func = request.Filter.ToFunc();
        return ValueTask.FromResult(AppResultList<Ticker>.Ok(_yard.Tickers.OrderBy(e => e.Symbol.Base).Where(func)));
    }
}