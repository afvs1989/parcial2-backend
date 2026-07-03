using Microsoft.EntityFrameworkCore;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;
using CineApi.Infrastructure.Data;

namespace CineApi.Infrastructure.Repositories;

public class PeliculaRepository(AppDbContext context) : IPeliculaRepository
{
    public async Task<IReadOnlyList<Pelicula>> ObtenerTodosAsync() =>
        await context.Peliculas
            .Include(p => p.Reparto)
            .ThenInclude(r => r.Actor)
            .OrderBy(p => p.Titulo)
            .ToListAsync();

    public async Task<Pelicula?> ObtenerPorIdAsync(int id) =>
        await context.Peliculas
            .Include(p => p.Reparto)
            .ThenInclude(r => r.Actor)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> ExisteTituloAsync(string titulo, int anio, int? excluirId = null) =>
        await context.Peliculas.AnyAsync(p =>
            p.Titulo == titulo.Trim() && p.Anio == anio && (!excluirId.HasValue || p.Id != excluirId));

    public async Task<Pelicula> CrearAsync(Pelicula pelicula)
    {
        context.Peliculas.Add(pelicula);
        await context.SaveChangesAsync();
        return pelicula;
    }

    public async Task ActualizarAsync(Pelicula pelicula)
    {
        context.Peliculas.Update(pelicula);
        await context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Pelicula pelicula)
    {
        context.Peliculas.Remove(pelicula);
        await context.SaveChangesAsync();
    }

    public async Task<Reparto?> ObtenerRepartoAsync(int peliculaId, int actorId) =>
        await context.Repartos
            .FirstOrDefaultAsync(r => r.PeliculaId == peliculaId && r.ActorId == actorId);

    public async Task AgregarRepartoAsync(Reparto reparto)
    {
        context.Repartos.Add(reparto);
        await context.SaveChangesAsync();
    }

    public async Task EliminarRepartoAsync(Reparto reparto)
    {
        context.Repartos.Remove(reparto);
        await context.SaveChangesAsync();
    }
}
