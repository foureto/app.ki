namespace App.Ki.Business.Services.Exchanges.Internals;

internal class ExchangeFactory : IExchangeFactory
{
    private readonly Func<string, IExchange> _getter;
    private readonly string[] _exchanges;

    public ExchangeFactory(Func<string, IExchange> getter)
    {
        _getter = getter;
        _exchanges = ExchangesInjections.Exchanges.Keys.ToArray();
    }

    public string[] GetExchanges() => _exchanges;

    public IExchange Get(string name) =>
        _getter(name) ?? throw new NullReferenceException($"No exchange with name {name}");

    public IExchange[] GetAll() => _exchanges.Select(_getter).ToArray();
}