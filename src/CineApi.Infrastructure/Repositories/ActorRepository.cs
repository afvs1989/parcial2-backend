using Microsoft.EntityFrameworkCore;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;
using CineApi.Infrastructure.Data;

namespace CineApi.Infrastructure.Repositories;

public class ActorRepository(AppDbContext context) : IActorRepository
{
    public async Task<IReadOnlyList<Actor>> ObtenerTodosAsync() =>
        await context.Actores
            .Include(a => a.Reparto)
            .ThenInclude(r => r.Pelicula)
            .OrderBy(a => a.Apellido)
            .ThenBy(a => a.Nombre)
            .ToListAsync();

    public async Task<Actor?> ObtenerPorIdAsync(int id) =>
        await context.Actores
            .Include(a => a.Reparto)
            .ThenInclude(r => r.Pelicula)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<bool> ExisteAsync(int id) =>
        await context.Actores.AnyAsync(a => a.Id == id);

    public async Task<bool> ExisteNombreAsync(string nombre, string apellido, int? excluirId = null) =>
        await context.Actores.AnyAsync(a =>
            a.Nombre == nombre.Trim() && a.Apellido == apellido.Trim() &&
            (!excluirId.HasValue || a.Id != excluirId));

    public async Task<Actor> CrearAsync(Actor actor)
    {
        context.Actores.Add(actor);
        await context.SaveChangesAsync();
        return actor;
    }

    public async Task ActualizarAsync(Actor actor)
    {
        context.Actores.Update(actor);
        await context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Actor actor)
    {
        context.Actores.Remove(actor);
        await context.SaveChangesAsync();
    }
}
