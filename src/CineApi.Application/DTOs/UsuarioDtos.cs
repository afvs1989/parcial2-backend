namespace CineApi.Application.DTOs;

public record UsuarioDto(int Id, string NombreUsuario, string Rol, bool Activo);

public record CrearUsuarioRequest(string NombreUsuario, string Password, string Rol, bool Activo = true);

public record ActualizarUsuarioRequest(string NombreUsuario, string? Password, string Rol, bool Activo);
