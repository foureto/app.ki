namespace App.Ki.Business.Services.Identity;

public interface ICurrentUserService<T>
{
    ICurrentUser<T> GetCurrentUser();
    Task SignInTemp(ICurrentUser<T> user, CancellationToken token = default);
    Task SignInGeneral(ICurrentUser<T> user, CancellationToken token = default);
}