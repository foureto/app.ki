using App.Ki.Business.Services.Identity;
using App.Ki.Commons.Models;
using Mediator;

namespace App.Ki.Business.Handlers.Identity;

public record GetMeQuery : IRequest<AppResult<ICurrentUser<int>>>;

public class GetMeHandler : IRequestHandler<GetMeQuery, AppResult<ICurrentUser<int>>>
{
    private readonly ICurrentUserService<int> _userService;

    public GetMeHandler(ICurrentUserService<int> userService)
    {
        _userService = userService;
    }
    
    public ValueTask<AppResult<ICurrentUser<int>>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(AppResult<ICurrentUser<int>>.Ok(_userService.GetCurrentUser()));
    }
}