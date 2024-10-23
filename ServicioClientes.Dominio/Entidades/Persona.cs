using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ServicioClientes.Dominio.Entidades;

[Table("Persona")]
public class Persona
{
    [Key]
    public int Id { get; set; }  
    public string Nombre { get; set; }
    public string? Genero { get; set; }
    public int Edad { get; set; }
    public string? Identificacion { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
}

