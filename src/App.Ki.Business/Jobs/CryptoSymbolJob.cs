using App.Ki.Business.Services.Exchanges;
using App.Ki.Business.Services.Feed;
using Microsoft.Extensions.Logging;
using Quartz;

namespace App.Ki.Business.Jobs;

[DisallowConcurrentExecution]
internal class CryptoSymbolJob : IJob
{
    private readonly IExchangeFactory _factory;
    private readonly IDataYard _yard;
    private readonly ILogger<CryptoSymbolJob> _logger;

    public CryptoSymbolJob(IExchangeFactory factory, IDataYard yard, ILogger<CryptoSymbolJob> logger)
    {
        _factory = factory;
        _yard = yard;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var exchanges = _factory.GetAll();
        foreach (var exchange in exchanges)
        {
            var pairs = await exchange.GetPairs();
            if (!pairs.Success)
            {
                _logger.LogWarning("Could not get Symbols from {Exchange}", exchange.GetType().Name);
                continue;
            }
            
            _yard.Enqueue(pairs.Data);
        }
    }
}