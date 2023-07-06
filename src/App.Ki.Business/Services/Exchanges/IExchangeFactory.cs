namespace App.Ki.Business.Services.Exchanges;

public interface IExchangeFactory
{
    string[] GetExchanges();
    IExchange Get(string name);
    IExchange[] GetAll();
}