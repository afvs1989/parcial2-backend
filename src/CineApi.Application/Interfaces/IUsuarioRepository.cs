using CineApi.Domain.Entities;

namespace CineApi.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorNombreAsync(string nombreUsuario);
    Task<IReadOnlyList<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNombreAsync(string nombreUsuario, int? excluirId = null);
    Task<Usuario> CrearAsync(Usuario usuario);
    Task ActualizarAsync(Usuario usuario);
    Task EliminarAsync(Usuario usuario);
}
