using App.Ki.Clickhouse;
using App.Ki.Clickhouse.Internals;

namespace App.Ki.DataAccess.History;

public class HistoryContext : ClickhouseContext
{
    public HistoryContext(ClickhouseConnectionFactory factory) : base(factory)
    {
    }
}