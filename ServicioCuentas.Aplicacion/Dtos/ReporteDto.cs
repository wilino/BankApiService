using System;
using ServicioClientes.Aplicacion.DTOs;

namespace ServicioCuentas.Aplicacion.Dtos
{

    public class ReporteDto
    {
        public string Fecha { get; set; } 
        public string Cliente { get; set; }  // Nombre del cliente
        public int NumeroCuenta { get; set; }  // Número de cuenta
        public string Tipo { get; set; }  // Tipo de cuenta (Corriente, Ahorros)
        public decimal SaldoInicial { get; set; }  
        public bool Estado { get; set; }  
        public decimal Movimiento { get; set; }  // Valor del movimiento (Depósito o Retiro)
        public decimal SaldoDisponible { get; set; }  
    }


}

