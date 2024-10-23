using Microsoft.AspNetCore.Mvc;
using ServicioClientes.Aplicacion.DTOs;
using ServicioCuentas.Aplicacion.DTOs;
using ServicioCuentas.Aplicacion.Parametros;
using ServicioCuentas.Aplicacion.Servicios.Imp;
using ServicioCuentas.Aplicacion.Servicios.Interfaces;

namespace ServicioCuentas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentaController : ControllerBase
    {
        private readonly ICuentaServicio _cuentaServicio;
        private readonly IMovimientoServicio _movimientoServicio;

        public CuentaController(ICuentaServicio cuentaServicio, IMovimientoServicio movimientoServicio)
        {
            _cuentaServicio = cuentaServicio;
            _movimientoServicio = movimientoServicio;
        }


        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CrearCuenta([FromBody] CrearCuentaParametros parametros)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cuentaCreada = await _cuentaServicio.CrearCuentaAsync(parametros);
                return CreatedAtAction(nameof(ObtenerCuentaPorId), new { cuentaId = cuentaCreada.NumeroCuenta }, cuentaCreada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }


        [HttpGet("{cuentaId}")]
        public async Task<IActionResult> ObtenerCuentaPorId(int cuentaId)
        {
            try
            {
                var cuenta = await _cuentaServicio.ObtenerCuentaPorIdAsync(cuentaId);
                if (cuenta == null)
                {
                    return NotFound(new { mensaje = "Cuenta no encontrada." });
                }
                return Ok(cuenta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al obtener la cuenta.", detalle = ex.Message });
            }
        }




        [HttpGet("cuentasPorCliente")]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> ObtenerCuentasPorCliente(int clienteId)
        {
            try
            {
                var cuentas = await _cuentaServicio.ObtenerCuentasPorClienteIdAsync(clienteId);
                if (cuentas == null || !cuentas.Any())
                {
                    return NotFound($"El cliente con ID {clienteId} no tiene cuentas asociadas.");
                }

                return Ok(cuentas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPut("{cuentaId}")]
        public async Task<IActionResult> ActualizarCuenta(int cuentaId, [FromBody] ActualizarCuentaParametros parametros)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cuentaActualizada = await _cuentaServicio.ActualizarCuentaAsync(cuentaId, parametros);
                if (cuentaActualizada == null)
                {
                    return NotFound(new { mensaje = "Cuenta no encontrada." });
                }

                return Ok(cuentaActualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar la cuenta.", detalle = ex.Message });
            }
        }


        [HttpDelete("{numeroCuenta}")]
        public async Task<IActionResult> EliminarCuenta(int numeroCuenta)
        {
            try
            {
              
                var cuenta = await _cuentaServicio.ObtenerCuentaPorNumeroCuentaAsync(numeroCuenta);
                if (cuenta == null)
                {
                    return NotFound(new { mensaje = "La cuenta no existe." });
                }

               
                var movimientos = await _movimientoServicio.ObtenerMovimientosPorCuentaAsync(cuenta.NumeroCuenta);
                if (movimientos.Any())
                {
                    return BadRequest(new { mensaje = "No se puede eliminar la cuenta porque tiene movimientos asociados." });
                }

               
                await _cuentaServicio.EliminarCuentaAsync(cuenta.NumeroCuenta);
                return Ok(new { mensaje = "Cuenta eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error al intentar eliminar la cuenta.", detalle = ex.Message });
            }
        }




    }
}


