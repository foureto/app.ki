using App.Ki.Commons;
using App.Ki.Handlers;
using App.Ki.Services;
using App.Ki.Services.Internals;
using Flour.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLogging();

// Add services to the container.

builder.Services
    .AddAuthentication(AppConstants.CookieScheme)
    .AddCookie(AppConstants.CookieScheme, ConfigureCookieOptions)
    .AddCookie(AppConstants.CookieTempScheme, ConfigureCookieOptions).Services
    .AddAuthorization(options =>
        options.AddPolicy(
            "temp", auth => auth.AddAuthenticationSchemes(AppConstants.CookieTempScheme).RequireAuthenticatedUser()))
    .AddSingleton<IIdentityStore, IdentityStore>()
    .AddScoped<IAppIdentity, AppMemoryIdentity>()
    .AddSpaStaticFiles(e => e.RootPath = "dist");

var app = builder.Build();

app
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(_ => { })
    .Use((ctx, next) =>
    {
        if (!ctx.Request.Path.StartsWithSegments("/api")) return next();
        ctx.Response.StatusCode = 404;
        return Task.CompletedTask;
    });

app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    if (builder.Environment.IsDevelopment())
        spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:5173");
});

app.MapHandlers();

app.Run();

static void ConfigureCookieOptions(CookieAuthenticationOptions options)
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.LoginPath = "/";
    options.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.FromResult(new { });
    };
}