namespace ServicioClientes.Aplicacion.DTOs
{
    public class PersonaDto
    {
        public int PersonaId { get; set; }   
        public string Nombre { get; set; }
        public string? Genero { get; set; }
        public int Edad { get; set; }
        public string? Identificacion { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }
}

