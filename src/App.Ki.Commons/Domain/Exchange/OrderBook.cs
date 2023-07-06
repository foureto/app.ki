namespace App.Ki.Commons.Domain.Exchange;

public record OrderBook(Symbol Symbol, PriceLevel[] Bids, PriceLevel[] Asks, DateTime Stamp);
public record PriceLevel(decimal Price, decimal Amount);