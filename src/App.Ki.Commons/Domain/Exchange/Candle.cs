using App.Ki.Commons.Enums;

namespace App.Ki.Commons.Domain.Exchange;

public record CandleSet(
    Symbol Symbol,
    TimeRange Range,
    Candle[] Candles);

public record Candle(
    DateTime Stamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume);