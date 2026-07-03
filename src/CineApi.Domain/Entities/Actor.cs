namespace CineApi.Domain.Entities;

public class Actor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }
    public string Nacionalidad { get; set; } = string.Empty;

    public ICollection<Reparto> Reparto { get; set; } = new List<Reparto>();
}
