using Moq;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;
using CineApi.Infrastructure.Services;

namespace CineApi.Tests;

public class PeliculaServiceTests
{
    private readonly Mock<IPeliculaRepository> _peliculaRepo = new();
    private readonly Mock<IActorRepository> _actorRepo = new();
    private readonly PeliculaService _service;

    public PeliculaServiceTests()
    {
        _service = new PeliculaService(_peliculaRepo.Object, _actorRepo.Object);
    }

    [Fact]
    public async Task CrearAsync_RetornaError_CuandoAnioEsInvalido()
    {
        var request = new CrearPeliculaRequest("Una película", "Drama", 1500, "Director Prueba");

        var (pelicula, error) = await _service.CrearAsync(request);

        Assert.Null(pelicula);
        Assert.Contains("año", error);
    }

    [Fact]
    public async Task CrearAsync_RetornaPelicula_CuandoDatosSonValidos()
    {
        var request = new CrearPeliculaRequest("Interstellar", "Ciencia ficción", 2014, "Christopher Nolan");

        _peliculaRepo.Setup(r => r.ExisteTituloAsync(It.IsAny<string>(), It.IsAny<int>(), null))
            .ReturnsAsync(false);
        _peliculaRepo.Setup(r => r.CrearAsync(It.IsAny<Pelicula>()))
            .ReturnsAsync((Pelicula p) =>
            {
                p.Id = 1;
                return p;
            });

        var (pelicula, error) = await _service.CrearAsync(request);

        Assert.Null(error);
        Assert.NotNull(pelicula);
        Assert.Equal("Interstellar", pelicula!.Titulo);
    }

    [Fact]
    public async Task AsignarActorAsync_RetornaError_CuandoActorNoExiste()
    {
        _peliculaRepo.Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(new Pelicula { Id = 1, Titulo = "Titanic", Genero = "Drama", Anio = 1997, Director = "James Cameron" });
        _actorRepo.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        var (pelicula, error) = await _service.AsignarActorAsync(1, new AsignarActorRequest(99, "Jack"));

        Assert.Null(pelicula);
        Assert.Equal("El actor seleccionado no existe.", error);
    }

    [Fact]
    public async Task AsignarActorAsync_RetornaError_CuandoActorYaEstaEnElReparto()
    {
        _peliculaRepo.Setup(r => r.ObtenerPorIdAsync(1))
            .ReturnsAsync(new Pelicula { Id = 1, Titulo = "Titanic", Genero = "Drama", Anio = 1997, Director = "James Cameron" });
        _actorRepo.Setup(r => r.ExisteAsync(5)).ReturnsAsync(true);
        _peliculaRepo.Setup(r => r.ObtenerRepartoAsync(1, 5))
            .ReturnsAsync(new Reparto { PeliculaId = 1, ActorId = 5, Personaje = "Jack" });

        var (pelicula, error) = await _service.AsignarActorAsync(1, new AsignarActorRequest(5, "Jack"));

        Assert.Null(pelicula);
        Assert.Equal("Ese actor ya está asignado a la película.", error);
    }
}
