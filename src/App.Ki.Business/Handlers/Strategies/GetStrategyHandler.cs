using App.Ki.Business.Handlers.Strategies.Models;
using App.Ki.Commons.Models;
using Mediator;

namespace App.Ki.Business.Handlers.Strategies;

public class GetStrategyQuery : IRequest<AppResult<StrategyDto>>
{
    public string Id { get; }

    public GetStrategyQuery(string id)
    {
        Id = id;
    }
}

public class GetStrategyHandler : IRequestHandler<GetStrategyQuery, AppResult<StrategyDto>>
{
    public ValueTask<AppResult<StrategyDto>> Handle(GetStrategyQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}