using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Infraestructura.Repositorios;
using ServicioCuentas.Aplicacion.Dtos;
using ServicioCuentas.Aplicacion.Parametros;
using ServicioCuentas.Aplicacion.Servicios.Interfaces;
using ServicioCuentas.Dominio.Entidades;
using ServicioCuentas.Dominio.Enums;

namespace ServicioCuentas.Aplicacion.Servicios.Imp
{
    public class MovimientoServicio : IMovimientoServicio
    {
        private readonly IRepositorioGenerico<Movimiento> _repositorioMovimiento;
        private readonly ICuentaServicio _cuentaServicio;

        public MovimientoServicio(IRepositorioGenerico<Movimiento> repositorioMovimiento,
                                  ICuentaServicio cuentaServicio)
        {
            _repositorioMovimiento = repositorioMovimiento;
            _cuentaServicio = cuentaServicio;
        }

        public async Task RegistrarMovimientoAsync(RegistrarMovimientoParametros parametros)
        {
            try
            {

                var cuenta = await _cuentaServicio.ObtenerCuentaPorNumeroCuentaAsync(parametros.NumeroCuenta);

                if (cuenta == null)
                {
                    throw new Exception("La cuenta no existe.");
                }


                var partesMovimiento = parametros.Movimiento.Split(' ');
                string tipoMovimientoTexto = partesMovimiento[0];
                decimal valorMovimiento = decimal.Parse(partesMovimiento[2]);


                TipoMovimiento tipoMovimiento;
                if (Enum.TryParse(tipoMovimientoTexto, true, out tipoMovimiento))
                {

                    if (tipoMovimiento == TipoMovimiento.Retiro && cuenta.SaldoInicial < valorMovimiento)
                    {
                        throw new Exception("Saldo no disponible.");
                    }


                    decimal nuevoSaldo = CalcularNuevoSaldo(cuenta.SaldoInicial, tipoMovimiento, valorMovimiento);


                    Movimiento nuevoMovimiento = new Movimiento
                    {
                        NumeroCuenta = parametros.NumeroCuenta,
                        Fecha = DateTime.Now,
                        TipoMovimiento = tipoMovimiento,
                        Valor = tipoMovimiento == TipoMovimiento.Retiro ? -valorMovimiento : valorMovimiento,  // Negativo para retiros
                        Saldo = nuevoSaldo,
                    };


                    cuenta.SaldoInicial = nuevoSaldo;


                    await _repositorioMovimiento.InsertarAsync(nuevoMovimiento);
                    await _repositorioMovimiento.GuardarAsync();
                    await _cuentaServicio.ActualizarCuentaAsync(cuenta);
                }
                else
                {
                    throw new Exception("Tipo de movimiento no válido.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo registrar el movimiento, intente mas tarde. " + ex.Message);
            }
        }

        private decimal CalcularNuevoSaldo(decimal saldoInicial, TipoMovimiento tipoMovimiento, decimal valorMovimiento)
        {
            return tipoMovimiento == TipoMovimiento.Retiro ? saldoInicial - valorMovimiento : saldoInicial + valorMovimiento;
        }




        public async Task<IEnumerable<MovimientoDto>> ObtenerMovimientosPorCuentaAsync(int cuentaId)
        {

            var cuenta = await _cuentaServicio.ObtenerCuentaPorNumeroCuentaAsync(cuentaId);
            if (cuenta == null)
            {
                throw new Exception("La cuenta no existe.");
            }


            var movimientos = await _repositorioMovimiento.BuscarAsync(m => m.NumeroCuenta == cuentaId);


            var movimientosDto = movimientos.Select(m => new MovimientoDto
            {
                NumeroCuenta = m.NumeroCuenta,
                TipoMovimiento = m.TipoMovimiento,
                Valor = m.Valor,
                Saldo = m.Saldo
            }).ToList();

            return movimientosDto;
        }

        public async Task<IEnumerable<MovimientoReporteDto>> ObtenerMovimientosPorFechaAsync(MovimientoParametros parametros)
        {

            var movimientos = await _repositorioMovimiento.BuscarAsync(m =>
                m.NumeroCuenta == parametros.NumeroCuenta &&
                m.Fecha >= parametros.FechaInicio &&
                m.Fecha <= parametros.FechaFin);


            return movimientos.Select(m => new MovimientoReporteDto
            {
                Fecha = m.Fecha.ToString("dd/MM/yyyy"),
                TipoMovimiento = m.TipoMovimiento.ToString(),
                Valor = m.Valor,
                SaldoDisponible = m.Saldo
            }).ToList();
        }
    }

}

