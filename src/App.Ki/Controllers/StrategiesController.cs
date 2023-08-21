using App.Ki.Business.Handlers.Strategies;
using App.Ki.Business.Handlers.Strategies.Models;
using App.Ki.Commons.Models.Paging;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace App.Ki.Controllers;

[Route("/api/strategies")]
public class StrategiesController: _BaseController
{
    private readonly IMediator _mediator;

    public StrategiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStrategy(string id)
        => Respond(await _mediator.Send(new GetStrategyQuery(id)));
    
    [HttpPost("page")]
    public async Task<IActionResult> GetStrategy([FromBody] PageContext<StrategyFilter> context)
        => Respond(await _mediator.Send(new GetStrategiesQuery(context)));
    
    [HttpPost]
    public async Task<IActionResult> AddStrategy([FromBody] AddStrategyDto strategy)
        => this.Respond(await _mediator.Send(new AddStrategyCommand(strategy)));
    
    [HttpPut("{id}")]
    public async Task<IActionResult> GetStrategy([FromBody] UpdateStrategyDto strategy)
        => Respond(await _mediator.Send(new UpdateStrategyCommand(strategy)));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStrategy(string id)
        => Respond(await _mediator.Send(new DeleteStrategyCommand(id)));
}