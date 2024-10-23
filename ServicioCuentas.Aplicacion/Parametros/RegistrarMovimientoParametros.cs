using System;
namespace ServicioCuentas.Aplicacion.Parametros
{
    public class RegistrarMovimientoParametros
    {
        public int NumeroCuenta { get; set; }
        public string Tipo { get; set; }  
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public string Movimiento { get; set; }  
    }

}

