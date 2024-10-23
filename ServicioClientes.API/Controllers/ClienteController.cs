using Microsoft.AspNetCore.Mvc;
using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Aplicacion.Parametros;
using ServicioClientes.Aplicacion.Servicios.Interfaces;

namespace ServicioClientes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteServicio _clienteServicio;

        public ClienteController(IClienteServicio clienteServicio)
        {
            _clienteServicio = clienteServicio;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> ObtenerTodosLosClientes()
        {
            var clientes = await _clienteServicio.ObtenerTodosLosClientesAsync();
            return Ok(clientes);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> ObtenerClientePorId(int id)
        {
            var cliente = await _clienteServicio.ObtenerClientePorIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return Ok(cliente);
        }

        
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CrearCliente([FromBody] CrearClienteParametros parametros)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var clienteCreado = await _clienteServicio.CrearClienteAsync(parametros);
                return CreatedAtAction(nameof(ObtenerClientePorId), new { id = clienteCreado.ClienteId }, clienteCreado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al crear el cliente. Por favor, inténtelo más tarde. {ex.Message}");
            }
        }


       
        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarCliente(int id, [FromBody] ActualizarClienteParametros parametros)
        {
            try
            {
                var clienteExistente = await _clienteServicio.ObtenerClientePorIdAsync(id);
                if (clienteExistente == null)
                {
                    return NotFound(new { mensaje = "El cliente no existe." });
                }

                await _clienteServicio.ActualizarClienteAsync(id, parametros);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar el cliente. Inténtelo más tarde.", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarCliente(int id)
        {
            try
            {
                var clienteExistente = await _clienteServicio.ObtenerClientePorIdAsync(id);
                if (clienteExistente == null)
                {
                    return NotFound();
                }

                await _clienteServicio.EliminarClienteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<ClienteDto>> ObtenerClientePorNombre(string nombre)
        {
            try
            {
                var cliente = await _clienteServicio.ObtenerClientePorNombreAsync(nombre);

                if (cliente == null)
                {
                    return NotFound($"No se encontró ningún cliente con el nombre: {nombre}");
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
