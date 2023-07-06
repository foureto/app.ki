namespace App.Ki.Business.Services.Exchanges.Models;

public class TickerInfo
{
    public string ApiSymbol { get; set; }
    public string Exchange { get; set; }
    public decimal Bid { get; set; }
    public decimal Ask { get; set; }
    public decimal BaseVolume { get; set; }
    public decimal QuotedVolume { get; set; }
    public DateTime Stamp { get; } = DateTime.UtcNow;
}