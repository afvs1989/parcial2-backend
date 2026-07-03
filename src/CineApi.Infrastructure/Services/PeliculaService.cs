using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;

namespace CineApi.Infrastructure.Services;

public class PeliculaService(
    IPeliculaRepository repository,
    IActorRepository actorRepository) : IPeliculaService
{
    private const int AnioMinimo = 1888;

    public async Task<IReadOnlyList<PeliculaDto>> ObtenerTodosAsync()
    {
        var peliculas = await repository.ObtenerTodosAsync();
        return peliculas.Select(MapToDto).ToList();
    }

    public async Task<PeliculaDto?> ObtenerPorIdAsync(int id)
    {
        var pelicula = await repository.ObtenerPorIdAsync(id);
        return pelicula is null ? null : MapToDto(pelicula);
    }

    public async Task<(PeliculaDto? Pelicula, string? Error)> CrearAsync(CrearPeliculaRequest request)
    {
        var error = Validar(request.Titulo, request.Genero, request.Anio, request.Director);
        if (error is not null)
            return (null, error);

        if (await repository.ExisteTituloAsync(request.Titulo, request.Anio))
            return (null, "Ya existe una película con ese título en ese mismo año.");

        var pelicula = new Pelicula
        {
            Titulo = request.Titulo.Trim(),
            Genero = request.Genero.Trim(),
            Anio = request.Anio,
            Director = request.Director.Trim()
        };

        var creada = await repository.CrearAsync(pelicula);
        return (MapToDto(creada), null);
    }

    public async Task<(PeliculaDto? Pelicula, string? Error)> ActualizarAsync(int id, ActualizarPeliculaRequest request)
    {
        var pelicula = await repository.ObtenerPorIdAsync(id);
        if (pelicula is null)
            return (null, "Película no encontrada.");

        var error = Validar(request.Titulo, request.Genero, request.Anio, request.Director);
        if (error is not null)
            return (null, error);

        if (await repository.ExisteTituloAsync(request.Titulo, request.Anio, id))
            return (null, "Ya existe otra película con ese título en ese mismo año.");

        pelicula.Titulo = request.Titulo.Trim();
        pelicula.Genero = request.Genero.Trim();
        pelicula.Anio = request.Anio;
        pelicula.Director = request.Director.Trim();

        await repository.ActualizarAsync(pelicula);
        return (MapToDto(pelicula), null);
    }

    public async Task<(bool Success, string? Error)> EliminarAsync(int id)
    {
        var pelicula = await repository.ObtenerPorIdAsync(id);
        if (pelicula is null)
            return (false, "Película no encontrada.");

        await repository.EliminarAsync(pelicula);
        return (true, null);
    }

    public async Task<(PeliculaDto? Pelicula, string? Error)> AsignarActorAsync(int peliculaId, AsignarActorRequest request)
    {
        var pelicula = await repository.ObtenerPorIdAsync(peliculaId);
        if (pelicula is null)
            return (null, "Película no encontrada.");

        if (!await actorRepository.ExisteAsync(request.ActorId))
            return (null, "El actor seleccionado no existe.");

        if (string.IsNullOrWhiteSpace(request.Personaje))
            return (null, "El nombre del personaje es obligatorio.");

        var existente = await repository.ObtenerRepartoAsync(peliculaId, request.ActorId);
        if (existente is not null)
            return (null, "Ese actor ya está asignado a la película.");

        await repository.AgregarRepartoAsync(new Reparto
        {
            PeliculaId = peliculaId,
            ActorId = request.ActorId,
            Personaje = request.Personaje.Trim()
        });

        var actualizada = await repository.ObtenerPorIdAsync(peliculaId);
        return (MapToDto(actualizada!), null);
    }

    public async Task<(bool Success, string? Error)> QuitarActorAsync(int peliculaId, int actorId)
    {
        var reparto = await repository.ObtenerRepartoAsync(peliculaId, actorId);
        if (reparto is null)
            return (false, "El actor no forma parte del reparto de la película.");

        await repository.EliminarRepartoAsync(reparto);
        return (true, null);
    }

    private static string? Validar(string titulo, string genero, int anio, string director)
    {
        if (string.IsNullOrWhiteSpace(titulo) || titulo.Trim().Length < 2)
            return "El título debe tener al menos 2 caracteres.";
        if (string.IsNullOrWhiteSpace(genero))
            return "El género es obligatorio.";
        if (anio < AnioMinimo || anio > DateTime.UtcNow.Year + 5)
            return $"El año debe estar entre {AnioMinimo} y {DateTime.UtcNow.Year + 5}.";
        if (string.IsNullOrWhiteSpace(director) || director.Trim().Length < 2)
            return "El director debe tener al menos 2 caracteres.";

        return null;
    }

    private static PeliculaDto MapToDto(Pelicula p) =>
        new(p.Id, p.Titulo, p.Genero, p.Anio, p.Director,
            p.Reparto
                .OrderBy(r => r.Actor?.Apellido)
                .Select(r => new RepartoDto(
                    r.ActorId,
                    r.Actor?.Nombre ?? string.Empty,
                    r.Actor?.Apellido ?? string.Empty,
                    r.Personaje))
                .ToList());
}
