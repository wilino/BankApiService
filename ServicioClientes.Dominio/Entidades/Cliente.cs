using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioClientes.Dominio.Entidades
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }  

        public string Contrasena { get; set; }
        public bool Estado { get; set; }


        public int PersonaId { get; set; }  
        public Persona Persona { get; set; }
    }
}

