using App.Ki.Commons.Models;
using Mediator;

namespace App.Ki.Business.Handlers.Strategies;

public class DeleteStrategyCommand : IRequest<AppResult>
{
    public string Id { get; }

    public DeleteStrategyCommand(string id)
    {
        Id = id;
    }
}

public class DeleteStrategyHandler : IRequestHandler<DeleteStrategyCommand, AppResult>
{
    public ValueTask<AppResult> Handle(DeleteStrategyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}