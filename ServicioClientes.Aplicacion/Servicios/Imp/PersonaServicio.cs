using ServicioClientes.Aplicacion.DTOs;
using ServicioClientes.Aplicacion.Parametros;
using ServicioClientes.Aplicacion.Servicios.Interfaces;
using ServicioClientes.Dominio.Entidades;
using ServicioClientes.Infraestructura.Repositorios;

namespace ServicioClientes.Aplicacion.Servicios.Imp
{
    public class PersonaServicio : IPersonaServicio
    {
        private readonly IRepositorioGenerico<Persona> _repositorioPersona;
        private readonly IClienteServicio _servicioCliente;

        public PersonaServicio(IRepositorioGenerico<Persona> repositorioPersona, IClienteServicio servicioCliente)
        {
            _repositorioPersona = repositorioPersona;
            _servicioCliente = servicioCliente;
        }

        public PersonaDto CrearPersona(CrearPersonaParametros parametros)
        {
            try
            {
                var persona = new Persona
                {
                    Nombre = parametros.Nombre,
                    Genero = parametros.Genero,
                    Edad = parametros.Edad,
                    Identificacion = parametros.Identificacion,
                    Direccion = parametros.Direccion,
                    Telefono = parametros.Telefono
                };

                _repositorioPersona.Insertar(persona);
                _repositorioPersona.Guardar();

                return new PersonaDto
                {
                    Nombre = persona.Nombre,
                    Genero = persona.Genero,
                    Edad = persona.Edad,
                    Identificacion = persona.Identificacion,
                    Direccion = persona.Direccion,
                    Telefono = persona.Telefono,
                    PersonaId = persona.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear la persona: {ex.Message}");
            }
        }

        public PersonaDto ObtenerPersonaPorId(int personaId)
        {
            try
            {
                var persona = _repositorioPersona.ObtenerPorId(personaId);
                if (persona != null)
                {
                    return new PersonaDto
                    {
                        PersonaId = persona.Id,
                        Nombre = persona.Nombre,
                        Genero = persona.Genero,
                        Edad = persona.Edad,
                        Identificacion = persona.Identificacion,
                        Direccion = persona.Direccion,
                        Telefono = persona.Telefono
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la persona: {ex.Message}");
            }
        }

        public IEnumerable<PersonaDto> ObtenerTodasLasPersonas()
        {
            try
            {
                var personas = _repositorioPersona.ObtenerTodos();
                return personas.Select(persona => new PersonaDto
                {
                    PersonaId = persona.Id,
                    Nombre = persona.Nombre,
                    Genero = persona.Genero,
                    Edad = persona.Edad,
                    Identificacion = persona.Identificacion,
                    Direccion = persona.Direccion,
                    Telefono = persona.Telefono
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todas las personas: {ex.Message}");
            }
        }

        public PersonaDto ActualizarPersona(int Id, ActualizarPersonaParametros parametros)
        {
            try
            {
                var personaExistente = _repositorioPersona.ObtenerPorId(Id);
                if (personaExistente == null)
                {
                    throw new Exception("La persona no existe.");
                }

                ActualizarCamposPersona(personaExistente, parametros);

                _repositorioPersona.Actualizar(personaExistente);
                _repositorioPersona.Guardar();

                return new PersonaDto
                {
                    PersonaId = personaExistente.Id,
                    Nombre = personaExistente.Nombre,
                    Genero = personaExistente.Genero,
                    Edad = personaExistente.Edad,
                    Identificacion = personaExistente.Identificacion,
                    Direccion = personaExistente.Direccion,
                    Telefono = personaExistente.Telefono
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la persona: " + ex.Message);
            }
        }

        private void ActualizarCamposPersona(Persona personaExistente, ActualizarPersonaParametros parametros)
        {
            if (!string.IsNullOrEmpty(parametros.Nombre))
            {
                personaExistente.Nombre = parametros.Nombre;
            }

            if (!string.IsNullOrEmpty(parametros.Genero))
            {
                personaExistente.Genero = parametros.Genero;
            }

            if (parametros.Edad.HasValue)
            {
                personaExistente.Edad = parametros.Edad.Value;
            }

            if (!string.IsNullOrEmpty(parametros.Identificacion))
            {
                personaExistente.Identificacion = parametros.Identificacion;
            }

            if (!string.IsNullOrEmpty(parametros.Direccion))
            {
                personaExistente.Direccion = parametros.Direccion;
            }

            if (!string.IsNullOrEmpty(parametros.Telefono))
            {
                personaExistente.Telefono = parametros.Telefono;
            }
        }



        public async Task EliminarPersonaAsync(int personaId)
        {
            try
            {
                var persona = _repositorioPersona.ObtenerPorId(personaId);
                if (persona == null)
                {
                    throw new Exception("La persona no existe.");
                }

                var clienteAsociado = await _servicioCliente.ObtenerClientePorIdPoridPersona(personaId);
                if (clienteAsociado != null)
                {
                    throw new Exception("No se puede eliminar la persona porque está asociada a un cliente.");
                }

                await _repositorioPersona.EliminarAsync(persona);
                await _repositorioPersona.GuardarAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la persona: {ex.Message}");
            }
        }

    }
}

