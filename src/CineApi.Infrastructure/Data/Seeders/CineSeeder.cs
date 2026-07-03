using Microsoft.EntityFrameworkCore;
using CineApi.Domain.Entities;

namespace CineApi.Infrastructure.Data.Seeders;

public static class CineSeeder
{
    private static readonly Actor[] ActoresIniciales =
    [
        new() { Nombre = "Leonardo", Apellido = "DiCaprio", FechaNacimiento = new DateOnly(1974, 11, 11), Nacionalidad = "Estadounidense" },
        new() { Nombre = "Kate", Apellido = "Winslet", FechaNacimiento = new DateOnly(1975, 10, 5), Nacionalidad = "Británica" },
        new() { Nombre = "Marion", Apellido = "Cotillard", FechaNacimiento = new DateOnly(1975, 9, 30), Nacionalidad = "Francesa" },
        new() { Nombre = "Tom", Apellido = "Hardy", FechaNacimiento = new DateOnly(1977, 9, 15), Nacionalidad = "Británico" },
        new() { Nombre = "Cillian", Apellido = "Murphy", FechaNacimiento = new DateOnly(1976, 5, 25), Nacionalidad = "Irlandés" },
        new() { Nombre = "Sofía", Apellido = "Vergara", FechaNacimiento = new DateOnly(1972, 7, 10), Nacionalidad = "Colombiana" }
    ];

    private static readonly Pelicula[] PeliculasIniciales =
    [
        new() { Titulo = "Titanic", Genero = "Drama", Anio = 1997, Director = "James Cameron" },
        new() { Titulo = "El Origen", Genero = "Ciencia ficción", Anio = 2010, Director = "Christopher Nolan" },
        new() { Titulo = "Oppenheimer", Genero = "Drama histórico", Anio = 2023, Director = "Christopher Nolan" }
    ];

    // Reparto: (Título película, Nombre actor, Apellido actor, Personaje)
    private static readonly (string Pelicula, string Nombre, string Apellido, string Personaje)[] RepartoInicial =
    [
        ("Titanic", "Leonardo", "DiCaprio", "Jack Dawson"),
        ("Titanic", "Kate", "Winslet", "Rose DeWitt Bukater"),
        ("El Origen", "Leonardo", "DiCaprio", "Dom Cobb"),
        ("El Origen", "Marion", "Cotillard", "Mal Cobb"),
        ("El Origen", "Tom", "Hardy", "Eames"),
        ("El Origen", "Cillian", "Murphy", "Robert Fischer"),
        ("Oppenheimer", "Cillian", "Murphy", "J. Robert Oppenheimer")
    ];

    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedActoresAsync(context);
        await SeedPeliculasAsync(context);
        await SeedRepartoAsync(context);
    }

    private static async Task SeedActoresAsync(AppDbContext context)
    {
        var existentes = await context.Actores
            .Select(a => a.Nombre + "|" + a.Apellido)
            .ToListAsync();

        var nuevos = ActoresIniciales
            .Where(a => !existentes.Contains(a.Nombre + "|" + a.Apellido))
            .ToList();

        if (nuevos.Count == 0)
            return;

        context.Actores.AddRange(nuevos);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPeliculasAsync(AppDbContext context)
    {
        var existentes = await context.Peliculas
            .Select(p => p.Titulo + "|" + p.Anio)
            .ToListAsync();

        var nuevas = PeliculasIniciales
            .Where(p => !existentes.Contains(p.Titulo + "|" + p.Anio))
            .ToList();

        if (nuevas.Count == 0)
            return;

        context.Peliculas.AddRange(nuevas);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRepartoAsync(AppDbContext context)
    {
        if (await context.Repartos.AnyAsync())
            return;

        var actores = await context.Actores.ToListAsync();
        var peliculas = await context.Peliculas.ToListAsync();

        var repartos = new List<Reparto>();
        foreach (var (tituloPelicula, nombre, apellido, personaje) in RepartoInicial)
        {
            var pelicula = peliculas.FirstOrDefault(p => p.Titulo == tituloPelicula);
            var actor = actores.FirstOrDefault(a => a.Nombre == nombre && a.Apellido == apellido);
            if (pelicula is null || actor is null)
                continue;

            repartos.Add(new Reparto
            {
                PeliculaId = pelicula.Id,
                ActorId = actor.Id,
                Personaje = personaje
            });
        }

        if (repartos.Count == 0)
            return;

        context.Repartos.AddRange(repartos);
        await context.SaveChangesAsync();
    }
}
