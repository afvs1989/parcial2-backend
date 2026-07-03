using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;

namespace CineApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuarios = await usuarioService.ObtenerTodosAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var usuario = await usuarioService.ObtenerPorIdAsync(id);
        if (usuario is null)
            return NotFound(new { message = "Usuario no encontrado." });

        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioRequest request)
    {
        var (usuario, error) = await usuarioService.CrearAsync(request);
        if (error is not null)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario!.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioRequest request)
    {
        var (usuario, error) = await usuarioService.ActualizarAsync(id, request);
        if (error == "Usuario no encontrado.")
            return NotFound(new { message = error });
        if (error is not null)
            return BadRequest(new { message = error });

        return Ok(usuario);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var (success, error) = await usuarioService.EliminarAsync(id);
        if (!success)
            return NotFound(new { message = error });

        return Ok(new { message = "Usuario eliminado correctamente." });
    }
}
