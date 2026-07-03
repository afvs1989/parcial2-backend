using Moq;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;
using CineApi.Domain.Entities;
using CineApi.Infrastructure.Services;

namespace CineApi.Tests;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock = new();
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _service = new UsuarioService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CrearAsync_RetornaError_CuandoNombreEsCorto()
    {
        var request = new CrearUsuarioRequest("ab", "Password1!", "Usuario");

        var (usuario, error) = await _service.CrearAsync(request);

        Assert.Null(usuario);
        Assert.Equal("El nombre de usuario debe tener al menos 3 caracteres.", error);
    }

    [Fact]
    public async Task CrearAsync_RetornaUsuario_CuandoDatosSonValidos()
    {
        var request = new CrearUsuarioRequest("nuevo.usuario", "Password1!", "Usuario");

        _repositoryMock.Setup(r => r.ExisteNombreAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.CrearAsync(It.IsAny<Usuario>()))
            .ReturnsAsync((Usuario u) =>
            {
                u.Id = 1;
                return u;
            });

        var (usuario, error) = await _service.CrearAsync(request);

        Assert.Null(error);
        Assert.NotNull(usuario);
        Assert.Equal("nuevo.usuario", usuario!.NombreUsuario);
    }

    [Fact]
    public async Task EliminarAsync_RetornaError_CuandoNoExiste()
    {
        _repositoryMock.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Usuario?)null);

        var (success, error) = await _service.EliminarAsync(99);

        Assert.False(success);
        Assert.Equal("Usuario no encontrado.", error);
    }
}
