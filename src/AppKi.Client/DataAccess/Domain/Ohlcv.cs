using App.Ki.Clickhouse.Attributes;

namespace AppKi.Client.DataAccess.Domain;

[ChTable("ohlcv")]
public class Ohlcv
{
    [ChField(Sort = 1, Type = "DateTime")] public DateTime Stamp { get; set; }
    [ChField] public decimal Open { get; set; }
    [ChField] public decimal High { get; set; }
    [ChField] public decimal Low { get; set; }
    [ChField] public decimal Close { get; set; }
    [ChField] public decimal Volume { get; set; }

    [ChField(Type = "varchar(30)", Sort = 2)]
    public string Symbol { get; set; } = string.Empty;

    [ChField(Type = "varchar(30)", Sort = 3)]
    public string Exchange { get; set; } = string.Empty;
}