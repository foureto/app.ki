namespace App.Ki.Business.Services.Exchanges.Models;

public class OrderBookInfo
{
    public string ApiSymbol { get; set; }
    public string Exchange { get; set; }
    public OrderBookEntry[] Bids { get; set; }
    public OrderBookEntry[] Asks { get; set; }
}

public class OrderBookEntry
{
    public double Price { get; set; }
    public double Quantity { get; set; }
}