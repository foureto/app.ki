namespace App.Ki.Commons.Domain.Exchange;

public record Ticker(
    Symbol Symbol,
    decimal Bid,
    decimal Ask,
    decimal Last, 
    decimal Lowest24H,
    decimal Highest24H,
    decimal ChangePercent,
    DateTime Stamp);