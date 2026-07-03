namespace CineApi.Domain.Entities;

public class Pelicula
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Director { get; set; } = string.Empty;

    public ICollection<Reparto> Reparto { get; set; } = new List<Reparto>();
}
