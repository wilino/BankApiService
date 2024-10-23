namespace ServicioCuentas.Aplicacion.DTOs
{
    public class CuentaDto
    {
        public int NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }  
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public string Cliente { get; set; }
        public int ClienteId { get; set; }
    }

}

