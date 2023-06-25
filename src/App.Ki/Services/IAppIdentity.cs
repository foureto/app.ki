using App.Ki.Commons.Models;
using App.Ki.Domain;

namespace App.Ki.Services;

public interface IAppIdentity
{
    // login
    Task<Result<string>> LoginPassword(string login, string password, CancellationToken token = default);
    Task<Result<string>> LoginPhone(string phone, string countryId, CancellationToken token = default);
    Task<Result<string>> LoginEmail(string email, CancellationToken token = default);
    Task<Result<string>> LoginExternal(string provider, CancellationToken token = default);
    Task<Result> ConfirmCode(string code, CancellationToken token = default);
    
    // logout
    Task<Result> Logout(CancellationToken token = default);
    
    // User info
    Task<Result<AppUser>> GetUserInfo(CancellationToken token = default);
}