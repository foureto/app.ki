using App.Ki.Business.Extensions;
using App.Ki.Commons.Domain.Exchange;

namespace App.Ki.Business.Models.Filters;

public class TickerFilterDto
{
    public string Exchange { get; set; }
    public string Currency { get; set; }

    public string ToKey() => this.ToJson();
    public Func<Ticker, bool> ToFunc()
        => e => (string.IsNullOrWhiteSpace(Exchange) || Exchange == e.Symbol.Exchange) &&
                (string.IsNullOrWhiteSpace(Currency) ||
                 Currency.Equals(e.Symbol.Base, StringComparison.OrdinalIgnoreCase) ||
                 Currency.Equals(e.Symbol.Quoted, StringComparison.OrdinalIgnoreCase));
}