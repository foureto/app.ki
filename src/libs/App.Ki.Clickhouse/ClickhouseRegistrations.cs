using App.Ki.Clickhouse.Internals;
using App.Ki.Clickhouse.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Ki.Clickhouse;

public static class ClickhouseRegistrations
{
    private const string DefaultSection = "clickhouse";

    public static IServiceCollection AddClickhouse(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSection)
    {
        services
            .Configure<ClickhouseSettings>(opts => configuration.GetSection(sectionName).Bind(opts))
            .AddHostedService<BufferFlusher>()
            .AddTransient<ClickhouseConnectionFactory>()
            .AddTransient<IMigrationService, MigrationService>()
            .AddTransient<ClickhouseContext>();

        return services;
    }
}