using Microsoft.EntityFrameworkCore;
using CineApi.Domain.Entities;

namespace CineApi.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Pelicula> Peliculas => Set<Pelicula>();
    public DbSet<Actor> Actores => Set<Actor>();
    public DbSet<Reparto> Repartos => Set<Reparto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.NombreUsuario).HasMaxLength(50).IsRequired();
            entity.HasIndex(u => u.NombreUsuario).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Rol).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Pelicula>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Titulo).HasMaxLength(150).IsRequired();
            entity.Property(p => p.Genero).HasMaxLength(60).IsRequired();
            entity.Property(p => p.Anio).IsRequired();
            entity.Property(p => p.Director).HasMaxLength(120).IsRequired();
            entity.HasIndex(p => new { p.Titulo, p.Anio }).IsUnique();
        });

        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Nombre).HasMaxLength(80).IsRequired();
            entity.Property(a => a.Apellido).HasMaxLength(80).IsRequired();
            entity.Property(a => a.FechaNacimiento).HasColumnType("date");
            entity.Property(a => a.Nacionalidad).HasMaxLength(60).IsRequired();
        });

        modelBuilder.Entity<Reparto>(entity =>
        {
            entity.HasKey(r => new { r.PeliculaId, r.ActorId });
            entity.Property(r => r.Personaje).HasMaxLength(120).IsRequired();

            entity.HasOne(r => r.Pelicula)
                .WithMany(p => p.Reparto)
                .HasForeignKey(r => r.PeliculaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Actor)
                .WithMany(a => a.Reparto)
                .HasForeignKey(r => r.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
