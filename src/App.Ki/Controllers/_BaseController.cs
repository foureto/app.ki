using App.Ki.Commons.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Ki.Controllers;

public class _BaseController : ControllerBase
{
    public IActionResult Respond(IAppResult result)
        => StatusCode(result.StatusCode, result);
}