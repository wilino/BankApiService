using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Aplicacion.Servicios.Interfaces;
using ServicioClientes.Dominio.Entidades;
using ServicioClientes.Infraestructura.Repositorios;
using ServicioClientes.Aplicacion.Parametros;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ServicioClientes.Aplicacion.Servicios.Imp
{
    public class ClienteServicio : IClienteServicio
    {
        private readonly IRepositorioGenerico<Cliente> _repositorioCliente;
        private readonly IRepositorioGenerico<Persona> _repositorioPersona;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ClienteServicio(IRepositorioGenerico<Cliente> repositorioCliente, IRepositorioGenerico<Persona> repositorioPersona,
                               HttpClient httpClient, IConfiguration configuration)
        {
            _repositorioCliente = repositorioCliente;
            _repositorioPersona = repositorioPersona;
            _httpClient = httpClient;
            _configuration = configuration;
        }


        public async Task<IEnumerable<ClienteDto>> ObtenerTodosLosClientesAsync()
        {
            try
            {
                var clientes = await _repositorioCliente.ObtenerTodosAsync(
                    include: query => query.Include(c => c.Persona)
                );

                return clientes.
                       Select(c => new ClienteDto
                       {
                           ClienteId = c.ClienteId,
                           PersonaId = c.PersonaId,
                           Nombre = c.Persona.Nombre,
                           Genero = c.Persona.Genero,
                           Identificacion = c.Persona.Identificacion,
                           Direccion = c.Persona.Direccion,
                           Edad = c.Persona.Edad,
                           Estado = c.Estado,
                           Telefono = c.Persona.Telefono,
                           Contrasena = c.Contrasena,
                       });
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener los clientes. Inténtelo más tarde.", ex);
            }
        }


        public async Task<ClienteDto> ObtenerClientePorIdAsync(int id)
        {
            var cliente = await ObtenerClientePorId(id);

            if (cliente == null)
            {
                throw new Exception("El cliente no existe.");
            }

            cliente.Persona = await _repositorioPersona.ObtenerPorIdAsync(cliente.PersonaId);

            return new ClienteDto
            {
                ClienteId = cliente.ClienteId,
                PersonaId = cliente.PersonaId,
                Nombre = cliente.Persona.Nombre,
                Genero = cliente.Persona.Genero,
                Identificacion = cliente.Persona.Identificacion,
                Direccion = cliente.Persona.Direccion,
                Edad = cliente.Persona.Edad,
                Estado = cliente.Estado,
                Telefono = cliente.Persona.Telefono,
                Contrasena = cliente.Contrasena,
            };
        }


        public async Task<ClienteDto> CrearClienteAsync(CrearClienteParametros parametros)
        {

            var clienteExistente = await GetClientePorDatos(parametros);
            if (clienteExistente != null)
            {
                throw new Exception("El cliente ya está registrado.");
            }


            var personaExistente = await _repositorioPersona.
                                   BuscarAsync(p => p.Nombre == parametros.Nombre &&
                                                    p.Direccion == parametros.Direccion &&
                                                    p.Telefono == parametros.Telefono);

            Cliente cliente = new Cliente();


            if (personaExistente.Any())
            {
                var persona = personaExistente.FirstOrDefault();
                cliente = new Cliente
                {
                    PersonaId = persona.Id,
                    Contrasena = parametros.Contrasena,
                    Estado = parametros.Estado
                };
            }
            else
            {

                var nuevaPersona = new Persona
                {
                    Nombre = parametros.Nombre,
                    Direccion = parametros.Direccion,
                    Telefono = parametros.Telefono,
                };

                await _repositorioPersona.InsertarAsync(nuevaPersona);
                await _repositorioPersona.GuardarAsync();

                cliente = new Cliente
                {
                    PersonaId = nuevaPersona.Id,
                    Contrasena = parametros.Contrasena,
                    Estado = parametros.Estado
                };
            }


            await _repositorioCliente.InsertarAsync(cliente);
            await _repositorioCliente.GuardarAsync();


            clienteExistente = await GetClientePorDatos(parametros);

            if (clienteExistente == null)
                throw new Exception("No se pudo registrar el cliente, intente mas tarde");

            return new ClienteDto
            {
                ClienteId = clienteExistente.ClienteId,
                Nombre = clienteExistente.Persona.Nombre,
                Direccion = clienteExistente.Persona.Direccion,
                Telefono = clienteExistente.Persona.Telefono,
                Contrasena = clienteExistente.Contrasena,
                Estado = clienteExistente.Estado,
                Genero = clienteExistente.Persona.Genero,
                Edad = clienteExistente.Persona.Edad,
                Identificacion = clienteExistente.Persona.Identificacion
            };
        }


        private async Task<Cliente> GetClientePorDatos(CrearClienteParametros parametros)
        {
            var clienteExistente = await _repositorioCliente
                .BuscarAsync(c => c.Persona.Nombre == parametros.Nombre &&
                                  c.Persona.Direccion == parametros.Direccion &&
                                  c.Persona.Telefono == parametros.Telefono);

            return clienteExistente.FirstOrDefault();
        }



        public async Task ActualizarClienteAsync(int clienteId, ActualizarClienteParametros parametros)
        {
            var clienteExistente = await ObtenerClientePorId(clienteId);

            ActualizarDatosCliente(clienteExistente, parametros);
            ActualizarDatosPersona(clienteExistente, parametros);

            await GuardarCambios(clienteExistente);
        }

        public async Task<Cliente> ObtenerClientePorId(int clienteId)
        {
            var cliente = await _repositorioCliente.ObtenerPorIdAsync(clienteId);
            if (cliente == null)
            {
                throw new Exception("El cliente no existe.");
            }
            return cliente;
        }

        private void ActualizarDatosCliente(Cliente clienteExistente, ActualizarClienteParametros parametros)
        {
            if (!string.IsNullOrEmpty(parametros.Contrasena))
            {
                clienteExistente.Contrasena = parametros.Contrasena;
            }

            if (parametros.Estado.HasValue)
            {
                clienteExistente.Estado = parametros.Estado.Value;
            }
        }

        private void ActualizarDatosPersona(Cliente clienteExistente, ActualizarClienteParametros parametros)
        {
            if (clienteExistente.Persona != null)
            {
                if (!string.IsNullOrEmpty(parametros.Nombre))
                {
                    clienteExistente.Persona.Nombre = parametros.Nombre;
                }

                if (!string.IsNullOrEmpty(parametros.Direccion))
                {
                    clienteExistente.Persona.Direccion = parametros.Direccion;
                }

                if (!string.IsNullOrEmpty(parametros.Telefono))
                {
                    clienteExistente.Persona.Telefono = parametros.Telefono;
                }

                if (!string.IsNullOrEmpty(parametros.Genero))
                {
                    clienteExistente.Persona.Genero = parametros.Genero;
                }

                if (parametros.Edad.HasValue)
                {
                    clienteExistente.Persona.Edad = parametros.Edad.Value;
                }

                if (!string.IsNullOrEmpty(parametros.Identificacion))
                {
                    clienteExistente.Persona.Identificacion = parametros.Identificacion;
                }
            }
        }

        private async Task GuardarCambios(Cliente clienteExistente)
        {
            await _repositorioCliente.ActualizarAsync(clienteExistente);
            await _repositorioCliente.GuardarAsync();
        }


        public async Task EliminarClienteAsync(int id)
        {
               var cliente = await _repositorioCliente.ObtenerPorIdAsync(id);
                if (cliente == null)
                {
                    throw new Exception("El cliente no existe.");
                }

                var cuentasAsociadas = await ObtenerCuentasAsociadasAsync(id);
                if (cuentasAsociadas.Any())
                {
                    throw new Exception($"El cliente no se puede eliminar porque tiene cuentas asociadas: {cuentasAsociadas.Count()}");
                }

                await EliminarClienteDeLaBaseDeDatosAsync(cliente);
        }

        private async Task<List<CuentaDto>> ObtenerCuentasAsociadasAsync(int clienteId)
        {
            try
            {
                var url = $"{_configuration["ServicioCuentasAPI:BaseUrl"]}cuentasPorCliente?clienteId={clienteId}";

                using (var respuestaCliente = await _httpClient.GetAsync(url))
                {
                    if (respuestaCliente.IsSuccessStatusCode)
                    {
                        var contenido = await respuestaCliente.Content.ReadAsStringAsync();

                        var cuentas = JsonConvert.DeserializeObject<List<CuentaDto>>(contenido);

                        return cuentas;
                    }
                    else
                    {
                        throw new Exception("Error al obtener las cuentas asociadas.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión al servicio de cuentas: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al obtener las cuentas asociadas: {ex.Message}");
            }
        }


        private async Task EliminarClienteDeLaBaseDeDatosAsync(Cliente cliente)
        {
            try
            {
                await _repositorioCliente.EliminarAsync(cliente);
                await _repositorioCliente.GuardarAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el cliente de la base de datos.", ex);
            }
        }



        public async Task<ClienteDto> ObtenerClientePorNombreAsync(string nombre)
        {
            var cliente = await _repositorioCliente
                            .BuscarAsync(c => c.Persona.Nombre == nombre);

            var clienteRes = cliente.FirstOrDefault();

            if (clienteRes == null)
            {
                return null;
            }

            if (clienteRes.Persona == null)
            {
                var persona = await _repositorioPersona.
                                    BuscarAsync(p => p.Id == clienteRes.PersonaId);
                clienteRes.Persona = persona.FirstOrDefault();
            }
            return new ClienteDto
            {
                ClienteId = clienteRes.ClienteId,
                Contrasena = clienteRes.Contrasena,
                Direccion = clienteRes.Persona.Direccion,
                Edad = clienteRes.Persona.Edad,
                Nombre = clienteRes.Persona.Nombre,
                Telefono = clienteRes.Persona.Telefono,
                Estado = clienteRes.Estado,
                Genero = clienteRes.Persona.Genero,
                PersonaId = clienteRes.Persona.Id,
                Identificacion = clienteRes.Persona.Identificacion
            };
        }

        public async Task<Cliente> ObtenerClientePorIdPoridPersona(int personaId)
        {
            try
            {

                var clienteRepo = await _repositorioCliente.BuscarAsync(c => c.PersonaId == personaId);
                var cliente = clienteRepo.FirstOrDefault();
                return cliente;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocurrió un error al intentar obtener el cliente con PersonaId {personaId}: {ex.Message}");
            }
        }
    }
}
