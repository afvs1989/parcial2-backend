using CineApi.Domain.Entities;

namespace CineApi.Application.Interfaces;

public interface IActorRepository
{
    Task<IReadOnlyList<Actor>> ObtenerTodosAsync();
    Task<Actor?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, string apellido, int? excluirId = null);
    Task<Actor> CrearAsync(Actor actor);
    Task ActualizarAsync(Actor actor);
    Task EliminarAsync(Actor actor);
}
