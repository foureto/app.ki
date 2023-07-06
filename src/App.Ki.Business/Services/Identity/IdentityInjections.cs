using System.Security.Claims;
using App.Ki.Business.Services.Identity.Internals;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace App.Ki.Business.Services.Identity;

internal static class IdentityInjections
{
    public static IServiceCollection AddAppIdentity(this IServiceCollection services)
    {
        return services
            .AddTransient<ICurrentUserService<int>, CurrentUserService>()
            .AddHttpContextAccessor()
            .AddAuthentication()
            .AddCookie(Constants.TempScheme, e => ConfigureCookieOptions(e, "localhost", "app.ki.tmp"))
            .AddCookie(Constants.GeneralScheme, e => ConfigureCookieOptions(e, "localhost", "app.ki"))
            .Services
            .AddAuthorization(options =>
            {
                ApplyPolicies(options, Constants.TempPolicy, Constants.TempScheme);
                ApplyPolicies(options, Constants.GeneralPolicy, Constants.GeneralScheme);
            })
            .AddDataProtection(opts => opts.ApplicationDiscriminator = "app.ki.identity").Services;
    }

    private static void ConfigureCookieOptions(
        CookieAuthenticationOptions options, string domain = "", string name = "")
    {
        options.SlidingExpiration = true;
        options.LoginPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.Cookie.Name = name ?? "app";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.FromResult(new { Message = "Not authorized", StatusCode = 401 });
        };

        if (!string.IsNullOrWhiteSpace(domain))
            options.Cookie.Domain = domain;
    }

    private static void ApplyPolicies(
        AuthorizationOptions options, string policyName, params string[] schemas)
    {
        options.AddPolicy(
            policyName,
            auth => auth
                .AddAuthenticationSchemes(schemas)
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.Role, Constants.Policies[policyName]));
    }
}