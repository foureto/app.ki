using App.Ki.Business.Services.Identity;
using App.Ki.Business.Services.Identity.Internals;
using App.Ki.Commons.Models;
using Mediator;

namespace App.Ki.Business.Handlers.Identity;

public record LoginCommand(string Login, string Password) : IRequest<AppResult>;

public class LoginHandler : IRequestHandler<LoginCommand, AppResult>
{
    private readonly ICurrentUserService<int> _userService;

    public LoginHandler(ICurrentUserService<int> userService)
    {
        _userService = userService;
    }

    public async ValueTask<AppResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        await _userService.SignInGeneral(new AppSessionUser { Id = 0, Name = "Name" }, cancellationToken);
        return AppResult.Ok();
    }
}