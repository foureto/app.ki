using App.Ki.Business.Handlers.Feed;
using App.Ki.Business.Models.Filters;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace App.Ki.Controllers;

[Route("/api/feed")]
public class FeedController : _BaseController
{
    private readonly IMediator _mediator;
    
    public FeedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("tickers")]
    public async Task<IActionResult> GetTickers([FromQuery] TickerFilterDto filter)
        => Respond(await _mediator.Send(new GetTickersQuery(filter)));
}