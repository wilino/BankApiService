using Microsoft.AspNetCore.Mvc;
using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Aplicacion.Parametros;
using ServicioClientes.Aplicacion.Servicios.Interfaces;

namespace ServicioClientes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly IPersonaServicio _personaServicio;

        public PersonaController(IPersonaServicio personaServicio)
        {
            _personaServicio = personaServicio;
        }

     
        [HttpGet]
        public ActionResult<IEnumerable<PersonaDto>> ObtenerTodasLasPersonas()
        {
            var personas = _personaServicio.ObtenerTodasLasPersonas();
            return Ok(personas);
        }

        
        [HttpGet("{id}")]
        public ActionResult<PersonaDto> ObtenerPersonaPorId(int id)
        {
            var persona = _personaServicio.ObtenerPersonaPorId(id);
            if (persona == null)
            {
                return NotFound();
            }
            return Ok(persona);
        }

        
        [HttpPost]
        public ActionResult<PersonaDto> CrearPersona([FromBody] CrearPersonaParametros parametros)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var personaCreada = _personaServicio.CrearPersona(parametros);
            return CreatedAtAction(nameof(ObtenerPersonaPorId), new { id = personaCreada.PersonaId }, personaCreada);
        }


        [HttpPut("{id}")]
        public ActionResult ActualizarPersona(int id, [FromBody] ActualizarPersonaParametros parametros)
        {
            try
            {
                _personaServicio.ActualizarPersona(id, parametros);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al actualizar la persona: {ex.Message}");
            }
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarPersona(int id)
        {
            try
            {
                await _personaServicio.EliminarPersonaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocurrió un error al eliminar la persona: {ex.Message}");
            }
        }

    }
}
