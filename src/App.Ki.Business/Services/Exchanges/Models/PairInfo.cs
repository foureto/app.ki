namespace App.Ki.Business.Services.Exchanges.Models;

public class PairInfo
{
    public string Base { get; set; }
    public string Quoted { get; set; }
    public string ApiSymbol { get; set; }
    public string Exchange { get; set; }
    public double BaseMinSize { get; set; }
    public double BaseMaxSize { get; set; }
    public double BaseIncrement { get; set; }
    public double QuotedMinSize { get; set; }
    public double QuotedMaxSize { get; set; }
    public double QuotedIncrement { get; set; }
    public double PriceIncrement { get; set; }
}