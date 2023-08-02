using App.Ki.Commons.Domain.Exchange;

namespace App.Ki.Business.Services.Exchanges.Models;

public class PairInfo
{
    public Symbol Symbol { get; set; }
    public decimal BaseMinSize { get; set; }
    public decimal BaseMaxSize { get; set; }
    public decimal BaseIncrement { get; set; }
    public decimal QuotedMinSize { get; set; }
    public decimal QuotedMaxSize { get; set; }
    public decimal QuotedIncrement { get; set; }
    public decimal PriceIncrement { get; set; }
}