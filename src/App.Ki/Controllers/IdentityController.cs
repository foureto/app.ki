using App.Ki.Business.Handlers.Identity;
using App.Ki.Business.Services.Identity;
using App.Ki.Commons.Models;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Ki.Controllers;

[Route("/api/identity")]
public class IdentityController : _BaseController
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command) 
        => Respond(command is not null ? await _mediator.Send(command) : AppResult.Bad("Invalid request"));
    
    [HttpGet("me")]
    [Authorize(Constants.GeneralPolicy)]
    public async Task<IActionResult> Me() 
        => Respond(await _mediator.Send(new GetMeQuery()));
}