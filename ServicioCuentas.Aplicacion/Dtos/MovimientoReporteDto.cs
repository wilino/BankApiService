using System;
namespace ServicioCuentas.Aplicacion.Dtos
{
    public class MovimientoReporteDto
    {
        public string Fecha { get; set; }  
        public string TipoMovimiento { get; set; }  
        public decimal Valor { get; set; }  
        public decimal SaldoDisponible { get; set; } 
    }

}

