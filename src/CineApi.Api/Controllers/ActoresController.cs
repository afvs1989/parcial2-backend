using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;

namespace CineApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActoresController(IActorService actorService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var actores = await actorService.ObtenerTodosAsync();
        return Ok(actores);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var actor = await actorService.ObtenerPorIdAsync(id);
        if (actor is null)
            return NotFound(new { message = "Actor no encontrado." });

        return Ok(actor);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] CrearActorRequest request)
    {
        var (actor, error) = await actorService.CrearAsync(request);
        if (error is not null)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = actor!.Id }, actor);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarActorRequest request)
    {
        var (actor, error) = await actorService.ActualizarAsync(id, request);
        if (error == "Actor no encontrado.")
            return NotFound(new { message = error });
        if (error is not null)
            return BadRequest(new { message = error });

        return Ok(actor);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var (success, error) = await actorService.EliminarAsync(id);
        if (!success)
            return NotFound(new { message = error });

        return Ok(new { message = "Actor eliminado correctamente." });
    }
}
