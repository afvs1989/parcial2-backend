namespace CineApi.Application.DTOs;

public record RepartoDto(
    int ActorId,
    string Nombre,
    string Apellido,
    string Personaje);

public record PeliculaDto(
    int Id,
    string Titulo,
    string Genero,
    int Anio,
    string Director,
    IReadOnlyList<RepartoDto> Reparto);

public record CrearPeliculaRequest(
    string Titulo,
    string Genero,
    int Anio,
    string Director);

public record ActualizarPeliculaRequest(
    string Titulo,
    string Genero,
    int Anio,
    string Director);

public record AsignarActorRequest(
    int ActorId,
    string Personaje);
