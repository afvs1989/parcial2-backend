using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;

namespace CineApi.Infrastructure.Services;

public class UsuarioService(IUsuarioRepository repository) : IUsuarioService
{
    private static readonly HashSet<string> RolesValidos = ["Administrador", "Usuario"];

    public async Task<IReadOnlyList<UsuarioDto>> ObtenerTodosAsync()
    {
        var usuarios = await repository.ObtenerTodosAsync();
        return usuarios.Select(MapToDto).ToList();
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
    {
        var usuario = await repository.ObtenerPorIdAsync(id);
        return usuario is null ? null : MapToDto(usuario);
    }

    public async Task<(UsuarioDto? Usuario, string? Error)> CrearAsync(CrearUsuarioRequest request)
    {
        var error = Validar(request.NombreUsuario, request.Password, request.Rol, esCreacion: true);
        if (error is not null)
            return (null, error);

        if (await repository.ExisteNombreAsync(request.NombreUsuario))
            return (null, "Ya existe un usuario con ese nombre.");

        var usuario = new Usuario
        {
            NombreUsuario = request.NombreUsuario.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rol = request.Rol.Trim(),
            Activo = request.Activo
        };

        var creado = await repository.CrearAsync(usuario);
        return (MapToDto(creado), null);
    }

    public async Task<(UsuarioDto? Usuario, string? Error)> ActualizarAsync(int id, ActualizarUsuarioRequest request)
    {
        var usuario = await repository.ObtenerPorIdAsync(id);
        if (usuario is null)
            return (null, "Usuario no encontrado.");

        var error = Validar(request.NombreUsuario, request.Password, request.Rol, esCreacion: false);
        if (error is not null)
            return (null, error);

        if (await repository.ExisteNombreAsync(request.NombreUsuario, id))
            return (null, "Ya existe otro usuario con ese nombre.");

        usuario.NombreUsuario = request.NombreUsuario.Trim();
        usuario.Rol = request.Rol.Trim();
        usuario.Activo = request.Activo;

        if (!string.IsNullOrWhiteSpace(request.Password))
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await repository.ActualizarAsync(usuario);
        return (MapToDto(usuario), null);
    }

    public async Task<(bool Success, string? Error)> EliminarAsync(int id)
    {
        var usuario = await repository.ObtenerPorIdAsync(id);
        if (usuario is null)
            return (false, "Usuario no encontrado.");

        await repository.EliminarAsync(usuario);
        return (true, null);
    }

    private static string? Validar(string nombreUsuario, string? password, string rol, bool esCreacion)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) || nombreUsuario.Trim().Length < 3)
            return "El nombre de usuario debe tener al menos 3 caracteres.";
        if (esCreacion && (string.IsNullOrWhiteSpace(password) || password.Length < 6))
            return "La contraseña debe tener al menos 6 caracteres.";
        if (!esCreacion && password is not null && password.Length > 0 && password.Length < 6)
            return "La contraseña debe tener al menos 6 caracteres.";
        if (!RolesValidos.Contains(rol.Trim()))
            return "Rol no válido.";

        return null;
    }

    private static UsuarioDto MapToDto(Usuario u) =>
        new(u.Id, u.NombreUsuario, u.Rol, u.Activo);
}
