namespace CineApi.Application.DTOs;

public record LoginRequest(string NombreUsuario, string Password);

public record LoginResponse(string NombreUsuario, string Rol);

public record AuthStatusResponse(bool Autenticado, string? NombreUsuario, string? Rol);
