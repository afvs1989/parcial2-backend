using CineApi.Domain.Entities;

namespace CineApi.Application.Interfaces;

public interface IPeliculaRepository
{
    Task<IReadOnlyList<Pelicula>> ObtenerTodosAsync();
    Task<Pelicula?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteTituloAsync(string titulo, int anio, int? excluirId = null);
    Task<Pelicula> CrearAsync(Pelicula pelicula);
    Task ActualizarAsync(Pelicula pelicula);
    Task EliminarAsync(Pelicula pelicula);

    Task<Reparto?> ObtenerRepartoAsync(int peliculaId, int actorId);
    Task AgregarRepartoAsync(Reparto reparto);
    Task EliminarRepartoAsync(Reparto reparto);
}
