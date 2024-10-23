namespace ServicioCuentas.Aplicacion.Dtos
{
    public class ClienteDto:PersonaDto
    {
        public int ClienteId { get; set; }
        public string Contrasena { get; set; }
        public bool Estado { get; set; }
    }
}

