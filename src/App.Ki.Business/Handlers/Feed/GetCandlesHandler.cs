using App.Ki.Business.Services.Exchanges;
using App.Ki.Commons.Domain.Exchange;
using App.Ki.Commons.Enums;
using App.Ki.Commons.Models;
using Mediator;
using Microsoft.Extensions.Logging;

namespace App.Ki.Business.Handlers.Feed;

public class GetCandlesFilterDto
{
    public string Exchange { get; set; }
    public string Symbol { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public TimeRange Range { get; set; } = TimeRange.ThirtyMinutes;
}

public class GetCandlesQuery : IRequest<AppResult<CandleSet>>
{
    public GetCandlesFilterDto Filter { get; }

    public GetCandlesQuery(GetCandlesFilterDto filter)
    {
        Filter = filter;
    }
}

public class GetCandlesHandler : IRequestHandler<GetCandlesQuery, AppResult<CandleSet>>
{
    private readonly IExchangeFactory _factory;
    private readonly ILogger<GetCandlesHandler> _logger;

    public GetCandlesHandler(IExchangeFactory factory, ILogger<GetCandlesHandler> logger)
    {
        _factory = factory;
        _logger = logger;
    }
    
    public async ValueTask<AppResult<CandleSet>> Handle(GetCandlesQuery request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var exchange = _factory.Get(request.Filter.Exchange);
        return AppResult<CandleSet>.Bad("not implemented");
    }
}