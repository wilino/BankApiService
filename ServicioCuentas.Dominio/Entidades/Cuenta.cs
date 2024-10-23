using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServicioCuentas.Dominio.Enums;
using Microsoft.EntityFrameworkCore;

namespace ServicioCuentas.Dominio.Entidades
{
    [Table("Cuenta")]
    [Index(nameof(NumeroCuenta), IsUnique = true)]
    public class Cuenta
    {
        [Key]
        public int Id { get; set; }
        public int NumeroCuenta { get; set; }
        public TipoCuenta TipoCuenta { get; set; } 
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public int ClienteId { get; set; }
    }
}

