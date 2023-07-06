using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace App.Ki.Business.Services.Identity.Internals;

internal class CurrentUserService : ICurrentUserService<int>
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public ICurrentUser<int> GetCurrentUser()
    {
        var user = _accessor.HttpContext?.User;

        if (user is null || (user.Identity?.IsAuthenticated ?? false))
            return new AppSessionUser();

        return new AppSessionUser
        {
            Id = int.Parse(user.FindFirst(e => e.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"),
            Name = user.FindFirst(e => e.Type == ClaimTypes.Name)?.Value
        };
    }

    public async Task SignInTemp(ICurrentUser<int> user, CancellationToken token = default)
    {
        var claims = new List<Claim>().ToList();
        claims.Add(new Claim(ClaimTypes.Role, claims.Find(e => e.Type == "role").Value));
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Constants.TempScheme));

        if (_accessor.HttpContext != null)
            await _accessor.HttpContext.SignInAsync(
                Constants.TempScheme, principal, new AuthenticationProperties { IsPersistent = true });
    }

    public async Task SignInGeneral(ICurrentUser<int> user, CancellationToken token = default)
    {
        var claims = new List<Claim>().Where(e => e.Type != "exp").ToList();
        claims.Add(new Claim(ClaimTypes.Role, claims.Find(e => e.Type == "role").Value));

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, Constants.GeneralScheme));

        if (_accessor.HttpContext != null)
        {
            await _accessor.HttpContext.SignOutAsync(Constants.TempScheme);
            await _accessor.HttpContext.SignInAsync(
                Constants.GeneralScheme, principal, new AuthenticationProperties { IsPersistent = true });
        }
    }
}