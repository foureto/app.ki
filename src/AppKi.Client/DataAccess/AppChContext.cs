using App.Ki.Clickhouse;
using App.Ki.Clickhouse.Internals;
using AppKi.Client.DataAccess.Domain;

namespace AppKi.Client.DataAccess;

public class AppChContext : ClickhouseContext
{
    public ChSet<Ohlcv> Ohlcvs { get; set; }
    
    public AppChContext(ClickhouseConnectionFactory factory) : base(factory)
    {
    }
}