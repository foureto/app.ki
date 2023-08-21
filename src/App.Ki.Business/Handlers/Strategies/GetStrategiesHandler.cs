using App.Ki.Business.Handlers.Strategies.Models;
using App.Ki.Commons.Models;
using App.Ki.Commons.Models.Paging;
using Mediator;

namespace App.Ki.Business.Handlers.Strategies;

public class GetStrategiesQuery : IRequest<PagedAppResult<StrategyDto>>
{
    public PageContext<StrategyFilter> Context { get; }

    public GetStrategiesQuery(PageContext<StrategyFilter> context)
    {
        Context = context;
    }
}

public class GetStrategiesHandler : IRequestHandler<GetStrategiesQuery, PagedAppResult<StrategyDto>>
{
    public ValueTask<PagedAppResult<StrategyDto>> Handle(
        GetStrategiesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}