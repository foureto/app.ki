using App.Ki.Commons.Domain.Exchange;

namespace App.Ki.Business.Services.Strategies;

public interface IProcessor
{
    bool GotEntry(List<Ticker> tickers);
}