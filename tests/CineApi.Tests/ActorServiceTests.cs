using Moq;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;
using CineApi.Infrastructure.Services;

namespace CineApi.Tests;

public class ActorServiceTests
{
    private readonly Mock<IActorRepository> _repositoryMock = new();
    private readonly ActorService _service;

    public ActorServiceTests()
    {
        _service = new ActorService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CrearAsync_RetornaError_CuandoNombreEsCorto()
    {
        var request = new CrearActorRequest("A", "Apellido", new DateOnly(1980, 1, 1), "Colombiana");

        var (actor, error) = await _service.CrearAsync(request);

        Assert.Null(actor);
        Assert.Equal("El nombre debe tener al menos 2 caracteres.", error);
    }

    [Fact]
    public async Task CrearAsync_RetornaActor_CuandoDatosSonValidos()
    {
        var request = new CrearActorRequest("Leonardo", "DiCaprio", new DateOnly(1974, 11, 11), "Estadounidense");

        _repositoryMock.Setup(r => r.ExisteNombreAsync(It.IsAny<string>(), It.IsAny<string>(), null))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.CrearAsync(It.IsAny<Actor>()))
            .ReturnsAsync((Actor a) =>
            {
                a.Id = 1;
                return a;
            });

        var (actor, error) = await _service.CrearAsync(request);

        Assert.Null(error);
        Assert.NotNull(actor);
        Assert.Equal("DiCaprio", actor!.Apellido);
    }

    [Fact]
    public async Task EliminarAsync_RetornaError_CuandoNoExiste()
    {
        _repositoryMock.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Actor?)null);

        var (success, error) = await _service.EliminarAsync(99);

        Assert.False(success);
        Assert.Equal("Actor no encontrado.", error);
    }
}
