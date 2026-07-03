using CineApi.Application.DTOs;

namespace CineApi.Application.Interfaces;

public interface IActorService
{
    Task<IReadOnlyList<ActorDto>> ObtenerTodosAsync();
    Task<ActorDto?> ObtenerPorIdAsync(int id);
    Task<(ActorDto? Actor, string? Error)> CrearAsync(CrearActorRequest request);
    Task<(ActorDto? Actor, string? Error)> ActualizarAsync(int id, ActualizarActorRequest request);
    Task<(bool Success, string? Error)> EliminarAsync(int id);
}
