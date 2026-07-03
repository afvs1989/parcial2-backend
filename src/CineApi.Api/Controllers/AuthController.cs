using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;

namespace CineApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, response, error) = await authService.LoginAsync(request);
        if (!success)
            return Unauthorized(new { message = error });

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Unauthorized(new { message = "No hay sesión activa." });

        await authService.LogoutAsync();
        return Ok(new { message = "Sesión cerrada correctamente." });
    }

    [HttpGet("status")]
    [AllowAnonymous]
    public async Task<IActionResult> Status()
    {
        var status = await authService.GetStatusAsync();
        return Ok(status);
    }
}
