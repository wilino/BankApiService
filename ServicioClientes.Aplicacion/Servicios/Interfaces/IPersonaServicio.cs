using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Aplicacion.Parametros;

namespace ServicioClientes.Aplicacion.Servicios.Interfaces
{
    public interface IPersonaServicio
    {
        IEnumerable<PersonaDto> ObtenerTodasLasPersonas();
        PersonaDto ObtenerPersonaPorId(int id); 
        PersonaDto CrearPersona(CrearPersonaParametros parametros);
        PersonaDto ActualizarPersona(int id, ActualizarPersonaParametros parametros);
        Task EliminarPersonaAsync(int personaId);
    }
}

