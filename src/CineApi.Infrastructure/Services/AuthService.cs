using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;

namespace CineApi.Infrastructure.Services;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    public const string SessionCookieName = "LoginSession";
    public const string SessionIdClaim = "SessionId";

    public async Task<(bool Success, LoginResponse? Response, string? Error)> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NombreUsuario) || string.IsNullOrWhiteSpace(request.Password))
            return (false, null, "Usuario y contraseña son obligatorios.");

        var usuario = await usuarioRepository.ObtenerPorNombreAsync(request.NombreUsuario.Trim());
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            return (false, null, "Credenciales inválidas.");

        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext no disponible.");

        // Regeneración de sesión: invalidar sesión previa antes de crear una nueva
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var sessionId = Guid.NewGuid().ToString("N");
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, usuario.NombreUsuario),
            new(ClaimTypes.Role, usuario.Rol),
            new(SessionIdClaim, sessionId)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            });

        return (true, new LoginResponse(usuario.NombreUsuario, usuario.Rol), null);
    }

    public async Task LogoutAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null)
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public Task<AuthStatusResponse> GetStatusAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var user = httpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            return Task.FromResult(new AuthStatusResponse(
                true,
                user.Identity?.Name,
                user.FindFirst(ClaimTypes.Role)?.Value));
        }

        return Task.FromResult(new AuthStatusResponse(false, null, null));
    }
}
