using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;

namespace CineApi.Infrastructure.Services;

public class ActorService(IActorRepository repository) : IActorService
{
    public async Task<IReadOnlyList<ActorDto>> ObtenerTodosAsync()
    {
        var actores = await repository.ObtenerTodosAsync();
        return actores.Select(MapToDto).ToList();
    }

    public async Task<ActorDto?> ObtenerPorIdAsync(int id)
    {
        var actor = await repository.ObtenerPorIdAsync(id);
        return actor is null ? null : MapToDto(actor);
    }

    public async Task<(ActorDto? Actor, string? Error)> CrearAsync(CrearActorRequest request)
    {
        var error = Validar(request.Nombre, request.Apellido, request.FechaNacimiento, request.Nacionalidad);
        if (error is not null)
            return (null, error);

        if (await repository.ExisteNombreAsync(request.Nombre, request.Apellido))
            return (null, "Ya existe un actor con ese nombre y apellido.");

        var actor = new Actor
        {
            Nombre = request.Nombre.Trim(),
            Apellido = request.Apellido.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            Nacionalidad = request.Nacionalidad.Trim()
        };

        var creado = await repository.CrearAsync(actor);
        return (MapToDto(creado), null);
    }

    public async Task<(ActorDto? Actor, string? Error)> ActualizarAsync(int id, ActualizarActorRequest request)
    {
        var actor = await repository.ObtenerPorIdAsync(id);
        if (actor is null)
            return (null, "Actor no encontrado.");

        var error = Validar(request.Nombre, request.Apellido, request.FechaNacimiento, request.Nacionalidad);
        if (error is not null)
            return (null, error);

        if (await repository.ExisteNombreAsync(request.Nombre, request.Apellido, id))
            return (null, "Ya existe otro actor con ese nombre y apellido.");

        actor.Nombre = request.Nombre.Trim();
        actor.Apellido = request.Apellido.Trim();
        actor.FechaNacimiento = request.FechaNacimiento;
        actor.Nacionalidad = request.Nacionalidad.Trim();

        await repository.ActualizarAsync(actor);
        return (MapToDto(actor), null);
    }

    public async Task<(bool Success, string? Error)> EliminarAsync(int id)
    {
        var actor = await repository.ObtenerPorIdAsync(id);
        if (actor is null)
            return (false, "Actor no encontrado.");

        await repository.EliminarAsync(actor);
        return (true, null);
    }

    private static string? Validar(string nombre, string apellido, DateOnly fechaNacimiento, string nacionalidad)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length < 2)
            return "El nombre debe tener al menos 2 caracteres.";
        if (string.IsNullOrWhiteSpace(apellido) || apellido.Trim().Length < 2)
            return "El apellido debe tener al menos 2 caracteres.";
        if (fechaNacimiento == default || fechaNacimiento > DateOnly.FromDateTime(DateTime.UtcNow))
            return "La fecha de nacimiento no es válida.";
        if (string.IsNullOrWhiteSpace(nacionalidad))
            return "La nacionalidad es obligatoria.";

        return null;
    }

    private static ActorDto MapToDto(Actor a) =>
        new(a.Id, a.Nombre, a.Apellido, a.FechaNacimiento, a.Nacionalidad,
            a.Reparto
                .OrderByDescending(r => r.Pelicula?.Anio)
                .Select(r => new PeliculaResumenDto(
                    r.PeliculaId,
                    r.Pelicula?.Titulo ?? string.Empty,
                    r.Pelicula?.Anio ?? 0,
                    r.Personaje))
                .ToList());
}
