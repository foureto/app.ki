namespace App.Ki.Business.Services.Exchanges.Models;

public class PairInfo
{
    public string Base { get; set; }
    public string Quoted { get; set; }
    public string ApiSymbol { get; set; }
    public string Exchange { get; set; }
    public decimal BaseMinSize { get; set; }
    public decimal BaseMaxSize { get; set; }
    public decimal BaseIncrement { get; set; }
    public decimal QuotedMinSize { get; set; }
    public decimal QuotedMaxSize { get; set; }
    public decimal QuotedIncrement { get; set; }
    public decimal PriceIncrement { get; set; }
}