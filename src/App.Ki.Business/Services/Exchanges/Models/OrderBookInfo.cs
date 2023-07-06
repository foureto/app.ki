namespace App.Ki.Business.Services.Exchanges.Models;

public class OrderBookInfo
{
    public string ApiSymbol { get; set; }
    public string Exchange { get; set; }
    public double[] Bids { get; set; }
    public double[] Asks { get; set; }
}