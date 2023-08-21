using App.Ki.Clickhouse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Ki.DataAccess;

public static class DataAccessInjections
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddClickhouse(configuration);
}