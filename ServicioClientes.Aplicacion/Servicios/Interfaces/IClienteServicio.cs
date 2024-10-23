using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Aplicacion.Parametros;
using ServicioClientes.Dominio.Entidades;

namespace ServicioClientes.Aplicacion.Servicios.Interfaces
{
    public interface IClienteServicio
    {
        Task<IEnumerable<ClienteDto>> ObtenerTodosLosClientesAsync();
        Task<ClienteDto> ObtenerClientePorIdAsync(int id);
        Task<ClienteDto> CrearClienteAsync(CrearClienteParametros parametros);
        Task ActualizarClienteAsync(int clienteId, ActualizarClienteParametros parametros);
        Task EliminarClienteAsync(int id);
        Task<ClienteDto> ObtenerClientePorNombreAsync(string nombre);
        Task<Cliente> ObtenerClientePorIdPoridPersona(int personaId);
    }
}

