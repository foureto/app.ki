namespace App.Ki.Commons.Domain.Exchange;

public record Symbol(string Base, string Quoted, string ApiSymbol, string Exchange)
{
    public override string ToString()
        => $"{ApiSymbol} at {Exchange}";
}