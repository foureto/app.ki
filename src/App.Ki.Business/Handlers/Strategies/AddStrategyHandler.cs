using App.Ki.Business.Handlers.Strategies.Models;
using App.Ki.Commons.Models;
using Mediator;

namespace App.Ki.Business.Handlers.Strategies;

public class AddStrategyCommand : IRequest<AppResult>
{
    public AddStrategyDto Strategy { get; }

    public AddStrategyCommand(AddStrategyDto strategy)
    {
        Strategy = strategy;
    }
}

public class AddStrategyHandler : IRequestHandler<AddStrategyCommand, AppResult>
{
    public ValueTask<AppResult> Handle(AddStrategyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}