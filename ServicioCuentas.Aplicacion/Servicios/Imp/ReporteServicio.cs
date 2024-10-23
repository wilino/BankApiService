using System;
using ServicioCuentas.Aplicacion.Dtos;
using ServicioCuentas.Aplicacion.Parametros;
using ServicioCuentas.Aplicacion.Servicios.Interfaces;

namespace ServicioCuentas.Aplicacion.Servicios.Imp
{
    public class ReporteServicio : IReporteServicio
    {
        private readonly ICuentaServicio _cuentaServicio;
        private readonly IMovimientoServicio _movimientoServicio;

        public ReporteServicio(ICuentaServicio cuentaServicio, IMovimientoServicio movimientoServicio)
        {
            _cuentaServicio = cuentaServicio;
            _movimientoServicio = movimientoServicio;
        }

        public async Task<IEnumerable<ReporteDto>> GenerarReporteAsync(ReporteCuentaParametros parametros)
        {
            try
            {
                var cuentas = await _cuentaServicio.ObtenerCuentasPorClienteAsync(parametros.Cliente);
                var reportes = new List<ReporteDto>();

                foreach (var cuenta in cuentas)
                {

                    var movimientoParametros = new MovimientoParametros
                    {
                        NumeroCuenta = cuenta.NumeroCuenta,
                        FechaInicio = parametros.FechaInicio,
                        FechaFin = parametros.FechaFin.AddHours(23).AddMinutes(59).AddSeconds(59)
                    };

                    var movimientos = await _movimientoServicio.ObtenerMovimientosPorFechaAsync(movimientoParametros);

                    foreach (var movimiento in movimientos)
                    {
                        var reporte = new ReporteDto
                        {
                            Fecha = movimiento.Fecha,
                            Cliente = parametros.Cliente,
                            NumeroCuenta = cuenta.NumeroCuenta,
                            Tipo = cuenta.TipoCuenta.ToString(),
                            SaldoInicial = DeterminaSaldoInicial(movimiento),
                            Estado = cuenta.Estado,
                            Movimiento = movimiento.Valor,
                            SaldoDisponible = movimiento.SaldoDisponible
                        };

                        reportes.Add(reporte);
                    }
                }

                return reportes;
            }

            catch (Exception ex)
            {
                throw new Exception($"Ocurrió un error al generar el reporte: {ex.Message}");
            }
        }

        private decimal DeterminaSaldoInicial(MovimientoReporteDto movimiento)
        {
            return movimiento.SaldoDisponible - movimiento.Valor;
        }
    }

}

