using App.Ki.Clickhouse.Internals;
using App.Ki.Clickhouse.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace App.Ki.Clickhouse;

public static class ClickhouseRegistrations
{
    private const string DefaultSection = "clickhouse";

    public static IServiceCollection AddClickhouse<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSection) where T : ClickhouseContext
    {
        return services
            .AddClickhouse(configuration, sectionName)
            .AddSingleton<T>();
    }
    
    public static IServiceCollection AddClickhouse(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSection)
    {
        services
            .Configure<ClickhouseSettings>(opts => configuration.GetSection(sectionName).Bind(opts))
            .AddHostedService<BufferFlusher>()
            .AddSingleton<ClickhouseConnectionFactory>()
            .AddTransient<IMigrationService, MigrationService>();

        return services;
    }
    
    
}