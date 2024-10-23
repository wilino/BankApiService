using System.Text.Json.Serialization;

namespace ServicioClientes.Aplicacion.DTOs
{
    public class ClienteDto:PersonaDto
    {
        public int ClienteId { get; set; }
        public string Contrasena { get; set; } 
        public bool Estado { get; set; }
    }
}

