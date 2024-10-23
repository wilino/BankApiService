using Microsoft.AspNetCore.Mvc;
using ServicioClientes.Aplicacion.DTOs;
using ServicioCuentas.Aplicacion.Parametros;
using ServicioCuentas.Aplicacion.Servicios.Interfaces;
using System.Threading.Tasks;

namespace ServicioCuentas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientoController : ControllerBase
    {
        private readonly IMovimientoServicio _movimientoServicio;

        public MovimientoController(IMovimientoServicio movimientoServicio)
        {
            _movimientoServicio = movimientoServicio;
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarMovimiento([FromBody] RegistrarMovimientoParametros parametros)
        {
            try
            {
                await _movimientoServicio.RegistrarMovimientoAsync(parametros);
                return Ok("Movimiento registrado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{cuentaId}")]
        public async Task<IActionResult> ObtenerMovimientosPorCuenta(int cuentaId)
        {
            try
            {
                var movimientos = await _movimientoServicio.ObtenerMovimientosPorCuentaAsync(cuentaId);
                if (movimientos == null)
                {
                    return NotFound("No se encontraron movimientos para la cuenta especificada.");
                }
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }


    }
}
