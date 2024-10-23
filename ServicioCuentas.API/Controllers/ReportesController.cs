using Microsoft.AspNetCore.Mvc;
using ServicioCuentas.Aplicacion.Parametros;
using ServicioCuentas.Aplicacion.Servicios.Interfaces;

namespace ServicioCuentas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteServicio _reporteServicio;

        public ReportesController(IReporteServicio reporteServicio)
        {
            _reporteServicio = reporteServicio;
        }

        [HttpGet]
        [Route("reportes")]
        public async Task<IActionResult> GenerarReporte([FromQuery] string fechaInicio, [FromQuery] string fechaFin, [FromQuery] string cliente)
        {
            try
            {
                // Parseamos las fechas que vienen en formato "dd/MM/yyyy"
                if (!DateTime.TryParseExact(fechaInicio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaInicioParsed))
                {
                    return BadRequest("El formato de fechaInicio no es válido. Debe ser dd/MM/yyyy.");
                }

                if (!DateTime.TryParseExact(fechaFin, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaFinParsed))
                {
                    return BadRequest("El formato de fechaFin no es válido. Debe ser dd/MM/yyyy.");
                }

                var parametros = new ReporteCuentaParametros
                {
                    Cliente = cliente,
                    FechaInicio = fechaInicioParsed,
                    FechaFin = fechaFinParsed
                };

                var reporte = await _reporteServicio.GenerarReporteAsync(parametros);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generando el reporte: {ex.Message}");
            }
        }
    }

}


