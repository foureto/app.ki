using System.Net.Mail;
using App.Ki.Commons.Models;
using App.Ki.Domain;

namespace App.Ki.Services.Internals;

internal class AppMemoryIdentity : IAppIdentity
{
    private readonly IHttpContextAccessor _accessor;

    public AppMemoryIdentity(
        IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
    
    public async Task<Result<string>> LoginPassword(string login, string password, CancellationToken token = default)
    {
        await Task.Yield();
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            return Result<string>.Bad("Invalid credentials");
        
        return Result<string>.Ok("123456");
    }

    public async Task<Result<string>> LoginPhone(string phone, string countryId, CancellationToken token = default)
    {
        await Task.Yield();
        if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(countryId))
            return Result<string>.Bad("Invalid credentials");
        
        return Result<string>.Ok("123456");
    }

    public async Task<Result<string>> LoginEmail(string email, CancellationToken token = default)
    {
        await Task.Yield();
        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out _))
            return Result<string>.Bad("Invalid credentials");
        
        return Result<string>.Ok("123456");
    }

    public async Task<Result<string>> LoginExternal(string provider, CancellationToken token = default)
    {
        await Task.Yield();
        return Result<string>.Ok("123456");
    }

    public async Task<Result> ConfirmCode(string code, CancellationToken token = default)
    {
        await Task.Yield();
        return Result.Ok();
    }

    public async Task<Result> Logout(CancellationToken token = default)
    {
        await Task.Yield();
        return Result.Ok();
    }

    public async Task<Result<AppUser>> GetUserInfo(CancellationToken token = default)
    {
        await Task.Yield();
        return Result<AppUser>.Ok(new AppUser());
    }
}