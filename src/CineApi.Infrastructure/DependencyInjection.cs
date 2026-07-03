using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CineApi.Application.Interfaces;
using CineApi.Infrastructure.Data;
using CineApi.Infrastructure.Data.Seeders;
using CineApi.Infrastructure.Repositories;
using CineApi.Infrastructure.Services;

namespace CineApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPeliculaRepository, PeliculaRepository>();
        services.AddScoped<IPeliculaService, PeliculaService>();
        services.AddScoped<IActorRepository, ActorRepository>();
        services.AddScoped<IActorService, ActorService>();

        return services;
    }

    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("CineApi.Seed");

        // SQL Server puede tardar en aceptar conexiones al arrancar (p. ej. en contenedor).
        // Reintentamos aplicar las migraciones antes de sembrar los datos.
        const int maxIntentos = 10;
        for (var intento = 1; intento <= maxIntentos; intento++)
        {
            try
            {
                logger.LogInformation("Aplicando migraciones (intento {Intento}/{Max})...", intento, maxIntentos);
                await context.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (intento < maxIntentos)
            {
                logger.LogWarning(ex, "La base de datos aún no está disponible. Reintentando en 3s...");
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }

        await UsuarioSeeder.SeedAsync(context);
        await CineSeeder.SeedAsync(context);

        var totalUsuarios = await context.Usuarios.CountAsync();
        var totalPeliculas = await context.Peliculas.CountAsync();
        var totalActores = await context.Actores.CountAsync();
        logger.LogInformation(
            "Seed completado. Usuarios: {Usuarios}, Películas: {Peliculas}, Actores: {Actores}. Login demo: admin / Admin123!",
            totalUsuarios, totalPeliculas, totalActores);
    }
}
