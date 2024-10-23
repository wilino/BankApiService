namespace ServicioCuentas.Aplicacion.Dtos
{
    public class ReporteCuentaDto
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public int NumeroCuenta { get; set; }
        public string Tipo { get; set; }
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public decimal Movimiento { get; set; }
        public decimal SaldoDisponible { get; set; }
    }

}

