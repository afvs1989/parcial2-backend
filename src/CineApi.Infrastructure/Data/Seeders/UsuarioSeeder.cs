using Microsoft.EntityFrameworkCore;
using CineApi.Domain.Entities;

namespace CineApi.Infrastructure.Data.Seeders;

public static class UsuarioSeeder
{
    private sealed record UsuarioSeed(string NombreUsuario, string Password, string Rol);

    private static readonly UsuarioSeed[] UsuariosIniciales =
    [
        new("admin", "Admin123!", "Administrador"),
        new("maria.garcia", "Maria2024!", "Usuario"),
        new("carlos.lopez", "Carlos2024!", "Usuario"),
        new("ana.rodriguez", "Ana2024!", "Administrador"),
        new("juan.perez", "Juan2024!", "Usuario")
    ];

    public static async Task SeedAsync(AppDbContext context)
    {
        var existentes = await context.Usuarios
            .Select(u => u.NombreUsuario)
            .ToListAsync();

        var nuevos = UsuariosIniciales
            .Where(seed => !existentes.Contains(seed.NombreUsuario))
            .Select(seed => new Usuario
            {
                NombreUsuario = seed.NombreUsuario,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(seed.Password),
                Rol = seed.Rol,
                Activo = true
            })
            .ToList();

        if (nuevos.Count == 0)
            return;

        context.Usuarios.AddRange(nuevos);
        await context.SaveChangesAsync();
    }
}
