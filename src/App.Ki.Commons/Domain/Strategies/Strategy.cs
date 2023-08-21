namespace App.Ki.Commons.Domain.Strategies;

public class Strategy
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public StrategySettings Settings { get; set; }
    public Arbitrage Arbitrage { get; set; }
}