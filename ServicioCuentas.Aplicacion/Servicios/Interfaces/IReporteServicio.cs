
using ServicioCuentas.Aplicacion.Dtos;
using ServicioCuentas.Aplicacion.Parametros;

namespace ServicioCuentas.Aplicacion.Servicios.Interfaces
{
    public interface IReporteServicio
    {
        Task<IEnumerable<ReporteDto>> GenerarReporteAsync(ReporteCuentaParametros parametros);
    }


}

