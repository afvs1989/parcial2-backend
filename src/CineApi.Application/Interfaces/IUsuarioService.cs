using CineApi.Application.DTOs;

namespace CineApi.Application.Interfaces;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioDto>> ObtenerTodosAsync();
    Task<UsuarioDto?> ObtenerPorIdAsync(int id);
    Task<(UsuarioDto? Usuario, string? Error)> CrearAsync(CrearUsuarioRequest request);
    Task<(UsuarioDto? Usuario, string? Error)> ActualizarAsync(int id, ActualizarUsuarioRequest request);
    Task<(bool Success, string? Error)> EliminarAsync(int id);
}
