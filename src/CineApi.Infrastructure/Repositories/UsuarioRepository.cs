using Microsoft.EntityFrameworkCore;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;
using CineApi.Infrastructure.Data;

namespace CineApi.Infrastructure.Repositories;

public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
{
    public async Task<Usuario?> ObtenerPorNombreAsync(string nombreUsuario) =>
        await context.Usuarios.FirstOrDefaultAsync(u =>
            u.NombreUsuario == nombreUsuario && u.Activo);

    public async Task<IReadOnlyList<Usuario>> ObtenerTodosAsync() =>
        await context.Usuarios.OrderBy(u => u.NombreUsuario).ToListAsync();

    public async Task<Usuario?> ObtenerPorIdAsync(int id) =>
        await context.Usuarios.FindAsync(id);

    public async Task<bool> ExisteNombreAsync(string nombreUsuario, int? excluirId = null) =>
        await context.Usuarios.AnyAsync(u =>
            u.NombreUsuario == nombreUsuario.Trim() && (!excluirId.HasValue || u.Id != excluirId));

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        return usuario;
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        context.Usuarios.Update(usuario);
        await context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Usuario usuario)
    {
        context.Usuarios.Remove(usuario);
        await context.SaveChangesAsync();
    }
}
