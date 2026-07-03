namespace CineApi.Domain.Entities;

/// <summary>
/// Relación muchos a muchos entre <see cref="Pelicula"/> y <see cref="Actor"/>.
/// Cada registro representa a un actor interpretando un personaje en una película.
/// </summary>
public class Reparto
{
    public int PeliculaId { get; set; }
    public Pelicula? Pelicula { get; set; }

    public int ActorId { get; set; }
    public Actor? Actor { get; set; }

    public string Personaje { get; set; } = string.Empty;
}
