
using System.Threading.Tasks;
using ServicioCuentas.Aplicacion.DTOs;
using ServicioCuentas.Aplicacion.Parametros;
using ServicioCuentas.Dominio.Entidades;

namespace ServicioCuentas.Aplicacion.Servicios.Interfaces
{
    public interface ICuentaServicio
    {
        Task<CuentaDto> CrearCuentaAsync(CrearCuentaParametros parametros);
        Task<CuentaDto> ObtenerCuentaPorIdAsync(int cuentaId);
        Task<IEnumerable<CuentaDto>> ObtenerCuentasPorClienteAsync(string nombreCliente);
        Task<Cuenta> ObtenerCuentaPorNumeroCuentaAsync(int numeroCuenta);
        Task ActualizarCuentaAsync(Cuenta cuenta);
        Task<IEnumerable<CuentaDto>> ObtenerCuentasPorClienteIdAsync(int clienteId);
        Task<CuentaDto> ActualizarCuentaAsync(int cuentaId, ActualizarCuentaParametros parametros);
        Task EliminarCuentaAsync(int numeroCuenta);
    }

}

