using App.Ki.Clickhouse;
using App.Ki.Clickhouse.Settings;
using AppKi.Client.DataAccess;
using AppKi.Client.Jobs;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddClickhouse<AppChContext>(builder.Configuration);
builder.Services.AddJobs(builder.Configuration);

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await Migrate(app, typeof(Program));

app.Run();

static async Task Migrate(WebApplication builder, params Type[] types)
{
    await using var scope = builder.Services.CreateAsyncScope();
    var settings = scope.ServiceProvider.GetRequiredService<IOptions<ClickhouseSettings>>();

    if (!settings.Value.UseMigrations) return;

    var migrator = scope.ServiceProvider.GetRequiredService<IMigrationService>();
    await migrator.Migrate(types.Select(e => e.Assembly));
}