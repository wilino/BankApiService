using System;
namespace ServicioCuentas.Aplicacion.Parametros
{
    public class CrearCuentaParametros
    {
        public int NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }  
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public string NombreCliente { get; set; }  
    }

}

