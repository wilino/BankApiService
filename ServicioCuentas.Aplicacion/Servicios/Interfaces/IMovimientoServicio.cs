using ServicioClientes.Aplicacion.DTOs;
using ServicioCuentas.Aplicacion.Dtos;
using ServicioCuentas.Aplicacion.Parametros;

namespace ServicioCuentas.Aplicacion.Servicios.Interfaces
{
    public interface IMovimientoServicio
    {
        Task RegistrarMovimientoAsync(RegistrarMovimientoParametros parametros);
        Task<IEnumerable<MovimientoDto>> ObtenerMovimientosPorCuentaAsync(int cuentaId);
        Task<IEnumerable<MovimientoReporteDto>> ObtenerMovimientosPorFechaAsync(MovimientoParametros parametros);


    }

}

