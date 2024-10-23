using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServicioCuentas.Dominio.Enums;

namespace ServicioCuentas.Dominio.Entidades
{
    [Table("Movimiento")]
    public class Movimiento
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }
        public int NumeroCuenta { get; set; }
    }
}

