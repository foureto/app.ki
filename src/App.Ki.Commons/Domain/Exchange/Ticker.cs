namespace App.Ki.Commons.Domain.Exchange;

public record Ticker(
    Symbol Symbol,
    decimal Bid,
    decimal Ask,
    decimal Last,
    DateTime Stamp);