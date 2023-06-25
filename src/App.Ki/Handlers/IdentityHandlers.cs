using System.Security.Claims;
using App.Ki.Commons;
using App.Ki.Commons.Models;
using App.Ki.Handlers.Models;
using App.Ki.Services;
using Microsoft.AspNetCore.Authentication;

namespace App.Ki.Handlers;

internal static class IdentityHandlers
{
    public static WebApplication AddIdentityHandlers(this WebApplication builder)
    {
        var group = builder.MapGroup("/api/identity");

        group.MapGet("me", async (HttpContext ctx, IIdentityStore store) =>
        {
            await ctx.Response.WriteAsJsonAsync(
                Result<UserDto>.Ok(new UserDto { Name = "%username%", Id = Guid.NewGuid().ToString("N") }));
        }).RequireAuthorization();

        group.MapPost("login", async (HttpContext ctx, IIdentityStore store) =>
        {
            var key = Guid.NewGuid().ToString("N");
            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, key) },
                    AppConstants.CookieTempScheme));

            await ctx.SignInAsync(
                AppConstants.CookieTempScheme, principal, new AuthenticationProperties { IsPersistent = true });
            await store.Store(Guid.NewGuid(), key);
            await ctx.Response.WriteAsJsonAsync(Result.Ok());
        });

        group.MapPost("login/code", async (LoginCodeDto code, HttpContext ctx, IIdentityStore store) =>
        {
            var key = ctx.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);
            if (key is null)
            {
                ctx.Response.StatusCode = 401;
                await ctx.Response.WriteAsJsonAsync(Result.UnAuthorized(string.Empty));
                return;
            }

            var id = await store.Get<Guid>(key.Value);

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) },
                    AppConstants.CookieScheme));

            await ctx.SignOutAsync(AppConstants.CookieTempScheme);
            await ctx.SignInAsync(
                AppConstants.CookieScheme, principal, new AuthenticationProperties { IsPersistent = true });
            await ctx.Response.WriteAsJsonAsync(Result.Ok());
        }).RequireAuthorization("temp");

        return builder;
    }
}