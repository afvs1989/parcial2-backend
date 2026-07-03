using CineApi.Application.DTOs;

namespace CineApi.Application.Interfaces;

public interface IPeliculaService
{
    Task<IReadOnlyList<PeliculaDto>> ObtenerTodosAsync();
    Task<PeliculaDto?> ObtenerPorIdAsync(int id);
    Task<(PeliculaDto? Pelicula, string? Error)> CrearAsync(CrearPeliculaRequest request);
    Task<(PeliculaDto? Pelicula, string? Error)> ActualizarAsync(int id, ActualizarPeliculaRequest request);
    Task<(bool Success, string? Error)> EliminarAsync(int id);
    Task<(PeliculaDto? Pelicula, string? Error)> AsignarActorAsync(int peliculaId, AsignarActorRequest request);
    Task<(bool Success, string? Error)> QuitarActorAsync(int peliculaId, int actorId);
}
