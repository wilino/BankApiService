using System;
using ServicioCuentas.Dominio.Enums;

namespace ServicioCuentas.Aplicacion.Parametros
{
	public class ActualizarCuentaParametros
	{
        public int? NumeroCuenta { get; set; }
        public TipoCuenta? TipoCuenta { get; set; }
        public decimal? SaldoInicial { get; set; }
        public bool? Estado { get; set; }
        public int? ClienteId { get; set; }
    }
}

