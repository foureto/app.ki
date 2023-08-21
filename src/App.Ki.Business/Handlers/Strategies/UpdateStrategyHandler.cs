using App.Ki.Business.Handlers.Strategies.Models;
using App.Ki.Commons.Models;
using Mediator;

namespace App.Ki.Business.Handlers.Strategies;

public class UpdateStrategyCommand : IRequest<AppResult>
{
    public UpdateStrategyDto Strategy { get; }

    public UpdateStrategyCommand(UpdateStrategyDto strategy)
    {
        Strategy = strategy;
    }
}

public class UpdateStrategyHandler : IRequestHandler<UpdateStrategyCommand, AppResult>
{
    public ValueTask<AppResult> Handle(UpdateStrategyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}