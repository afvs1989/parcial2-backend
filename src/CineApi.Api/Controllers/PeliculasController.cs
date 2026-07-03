using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineApi.Application.DTOs;
using CineApi.Application.Interfaces;

namespace CineApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PeliculasController(IPeliculaService peliculaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var peliculas = await peliculaService.ObtenerTodosAsync();
        return Ok(peliculas);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var pelicula = await peliculaService.ObtenerPorIdAsync(id);
        if (pelicula is null)
            return NotFound(new { message = "Película no encontrada." });

        return Ok(pelicula);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] CrearPeliculaRequest request)
    {
        var (pelicula, error) = await peliculaService.CrearAsync(request);
        if (error is not null)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = pelicula!.Id }, pelicula);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarPeliculaRequest request)
    {
        var (pelicula, error) = await peliculaService.ActualizarAsync(id, request);
        if (error == "Película no encontrada.")
            return NotFound(new { message = error });
        if (error is not null)
            return BadRequest(new { message = error });

        return Ok(pelicula);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var (success, error) = await peliculaService.EliminarAsync(id);
        if (!success)
            return NotFound(new { message = error });

        return Ok(new { message = "Película eliminada correctamente." });
    }

    [HttpPost("{id:int}/reparto")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AsignarActor(int id, [FromBody] AsignarActorRequest request)
    {
        var (pelicula, error) = await peliculaService.AsignarActorAsync(id, request);
        if (error == "Película no encontrada.")
            return NotFound(new { message = error });
        if (error is not null)
            return BadRequest(new { message = error });

        return Ok(pelicula);
    }

    [HttpDelete("{id:int}/reparto/{actorId:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> QuitarActor(int id, int actorId)
    {
        var (success, error) = await peliculaService.QuitarActorAsync(id, actorId);
        if (!success)
            return NotFound(new { message = error });

        return Ok(new { message = "Actor retirado del reparto correctamente." });
    }
}
