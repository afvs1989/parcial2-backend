namespace CineApi.Application.DTOs;

public record PeliculaResumenDto(
    int PeliculaId,
    string Titulo,
    int Anio,
    string Personaje);

public record ActorDto(
    int Id,
    string Nombre,
    string Apellido,
    DateOnly FechaNacimiento,
    string Nacionalidad,
    IReadOnlyList<PeliculaResumenDto> Peliculas);

public record CrearActorRequest(
    string Nombre,
    string Apellido,
    DateOnly FechaNacimiento,
    string Nacionalidad);

public record ActualizarActorRequest(
    string Nombre,
    string Apellido,
    DateOnly FechaNacimiento,
    string Nacionalidad);
