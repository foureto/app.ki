using App.Ki.Business;
using App.Ki.Business.Hubs;
using Flour.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLogging();

// Add services to the container.

builder.Services
    .AddControllers().Services
    .AddBusiness(builder.Configuration)
    .AddSpaStaticFiles(e => e.RootPath = "dist");

var app = builder.Build();

app
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(_ => { })
    .Use((ctx, next) =>
    {
        if (!ctx.Request.Path.StartsWithSegments("/api") || 
            !ctx.Request.Path.StartsWithSegments("/ws"))
            return next();
        ctx.Response.StatusCode = 404;
        return Task.CompletedTask;
    });

app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    if (builder.Environment.IsDevelopment())
        spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:5173");
});

app.MapControllers();
app.MapHub<FeedHub>("/ws/feed");
app.Run();