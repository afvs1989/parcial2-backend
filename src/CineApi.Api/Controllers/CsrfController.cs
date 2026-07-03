using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace CineApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class CsrfController(IAntiforgery antiforgery) : ControllerBase
{
    [HttpGet("token")]
    public IActionResult GetToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { token = tokens.RequestToken });
    }
}
