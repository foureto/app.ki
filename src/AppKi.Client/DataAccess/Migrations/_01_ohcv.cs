using App.Ki.Clickhouse;
using App.Ki.Clickhouse.Internals;
using AppKi.Client.DataAccess.Domain;

namespace AppKi.Client.DataAccess.Migrations;

public class _01_ohcv : ChMigration
{
    public override async ValueTask<bool> Up(ClickhouseSession session, IServiceProvider provider)
    {
        var table = GetTableMetadata<Ohlcv>();
        var query = GetTableCtor<Ohlcv>(session);
        await session.Run($"drop table if exists {table.Name}");
        await session.Run(query);
        return true;
    }

    public override ValueTask<bool> Down(ClickhouseSession session, IServiceProvider provider)
    {
        return ValueTask.FromResult(false);
    }
}