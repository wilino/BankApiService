using ServicioCuentas.Dominio.Enums;

namespace ServicioClientes.Aplicacion.DTOs
{
    public class MovimientoDto
    {
        public int NumeroCuenta { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; } 
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }
    }
}

