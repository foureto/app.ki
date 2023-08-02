using Tinkoff.InvestApi.V1;

namespace App.Ki.Business.Services.Exchanges;

public static class Extensions
{
    public static decimal ToDecimal(this Quotation value)
        => value.Units + (decimal)value.Nano / 1_000_000_000;
    
    public static decimal ToDecimal(this MoneyValue value)
        => value.Units + (decimal)value.Nano / 1_000_000_000;
}