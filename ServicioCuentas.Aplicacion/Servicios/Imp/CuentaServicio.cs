using Microsoft.Extensions.Configuration;
using ServicioClientes.Infraestructura.Repositorios;
using ServicioCuentas.Aplicacion.Dtos;
using ServicioCuentas.Aplicacion.Servicios.Interfaces;
using ServicioCuentas.Dominio.Entidades;
using ServicioCuentas.Dominio.Enums;
using Newtonsoft.Json;
using ServicioCuentas.Aplicacion.Parametros;
using System.Net.Http.Json;
using ServicioCuentas.Aplicacion.DTOs;

namespace ServicioCuentas.Aplicacion.Servicios.Imp
{
    public class CuentaServicio : ICuentaServicio
    {
        private readonly IRepositorioGenerico<Cuenta> _repositorioCuenta;
        private readonly HttpClient _httpClient;
        private readonly string _servicioUsuariosBaseUrl;

        public CuentaServicio(IRepositorioGenerico<Cuenta> repositorioCuenta, HttpClient httpClient, IConfiguration configuration)
        {
            _repositorioCuenta = repositorioCuenta;
            _httpClient = httpClient;
            _servicioUsuariosBaseUrl = configuration["ServicioUsuariosAPI:BaseUrl"];  // Obtenemos la URL desde appsettings.json
        }


        public async Task<CuentaDto> CrearCuentaAsync(CrearCuentaParametros parametros)
        {

            ClienteDto clienteDto;
            try
            {
                clienteDto = await ObtenerClientePorNombreAsync(parametros.NombreCliente);
                if (clienteDto == null)
                {
                    throw new Exception("El cliente no existe o no se pudo comunicar con el servicio de usuarios.");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error al intentar obtener los datos del cliente: " + ex.Message);
            }

            try
            {
                var cuenta = new Cuenta
                {
                    NumeroCuenta = parametros.NumeroCuenta,
                    TipoCuenta = Enum.Parse<TipoCuenta>(parametros.TipoCuenta, true),
                    SaldoInicial = parametros.SaldoInicial,
                    Estado = parametros.Estado,
                    ClienteId = clienteDto.ClienteId
                };

                await _repositorioCuenta.InsertarAsync(cuenta);
                await _repositorioCuenta.GuardarAsync();

                return new CuentaDto
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = cuenta.TipoCuenta.ToString(),
                    SaldoInicial = cuenta.SaldoInicial,
                    Estado = cuenta.Estado,
                    ClienteId = clienteDto.ClienteId,
                    Cliente = clienteDto.Nombre
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear cuenta: " + ex.Message);
            }
        }


        private async Task<ClienteDto> ObtenerClientePorNombreAsync(string nombreCliente)
        {
            try
            {

                var url = $"{_servicioUsuariosBaseUrl}nombre/{nombreCliente}";


                using (var respuestaCliente = await _httpClient.GetAsync(url))
                {
                    if (!respuestaCliente.IsSuccessStatusCode)
                    {

                        throw new HttpRequestException($"Error al obtener cliente: {respuestaCliente.StatusCode}");
                    }

                    var clienteDto = await respuestaCliente.Content.ReadFromJsonAsync<ClienteDto>();

                    return clienteDto;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error en la solicitud al servicio de clientes: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al intentar obtener el cliente: {ex.Message}");
            }
        }


        public async Task<CuentaDto> ObtenerCuentaPorIdAsync(int cuentaId)
        {
            var cuenta = await _repositorioCuenta.ObtenerPorIdAsync(cuentaId);
            if (cuenta == null)
            {
                throw new Exception("La cuenta no existe.");
            }


            var nombreCliente = await ObtenerNombreClientePorId(cuenta.ClienteId);
            if (string.IsNullOrEmpty(nombreCliente))
            {
                throw new Exception("No se pudo obtener el nombre del cliente.");
            }


            return new CuentaDto
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta.ToString(),
                SaldoInicial = cuenta.SaldoInicial,
                Estado = cuenta.Estado,
                Cliente = nombreCliente,
                ClienteId=cuenta.ClienteId
            };
        }


        private async Task<string> ObtenerNombreClientePorId(int clienteId)
        {
            var response = await _httpClient.GetAsync($"{_servicioUsuariosBaseUrl}{clienteId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var cliente = JsonConvert.DeserializeObject<ClienteDto>(jsonString);
                return cliente?.Nombre;
            }

            return null;
        }


        public async Task<IEnumerable<CuentaDto>> ObtenerCuentasPorClienteAsync(string nombreCliente)
        {

            var cliente = await ObtenerClientePorNombreAsync(nombreCliente);


            if (cliente == null)
            {
                throw new Exception($"El cliente con nombre {nombreCliente} no existe.");
            }


            var cuentas = await _repositorioCuenta.BuscarAsync(c => c.ClienteId == cliente.ClienteId);

            if (!cuentas.Any())
            {
                throw new Exception($"El cliente {nombreCliente} no tiene cuentas asociadas.");
            }


            return cuentas.Select(c => new CuentaDto
            {
                NumeroCuenta = c.NumeroCuenta,
                TipoCuenta = c.TipoCuenta.ToString(),
                SaldoInicial = c.SaldoInicial,
                Estado = c.Estado,
                ClienteId = c.ClienteId
            }).ToList();
        }

        public async Task<Cuenta> ObtenerCuentaPorNumeroCuentaAsync(int numeroCuenta)
        {
            var cuentaRepo = await _repositorioCuenta.BuscarAsync(c => c.NumeroCuenta == numeroCuenta);
            return cuentaRepo.FirstOrDefault();
        }

        public async Task ActualizarCuentaAsync(Cuenta cuenta)
        {
            await _repositorioCuenta.ActualizarAsync(cuenta);
            await _repositorioCuenta.GuardarAsync();
        }

        public async Task<IEnumerable<CuentaDto>> ObtenerCuentasPorClienteIdAsync(int clienteId)
        {
            var cuentas = await _repositorioCuenta.BuscarAsync(c => c.ClienteId == clienteId);

            if (!cuentas.Any())
            {
                throw new Exception($"El cliente con ID {clienteId} no tiene cuentas asociadas.");
            }

            return cuentas.Select(c => new CuentaDto
            {
                NumeroCuenta = c.NumeroCuenta,
                TipoCuenta = c.TipoCuenta.ToString(),
                SaldoInicial = c.SaldoInicial,
                Estado = c.Estado,
                ClienteId = c.ClienteId
            }).ToList();
        }

        public async Task<CuentaDto> ActualizarCuentaAsync(int cuentaId, ActualizarCuentaParametros parametros)
        {
          
            var cuenta = await _repositorioCuenta.ObtenerPorIdAsync(cuentaId);

            if (cuenta == null)
            {
                throw new Exception("La cuenta no existe.");
            }

           
            ActualizarCamposCuenta(cuenta, parametros);

            
            await _repositorioCuenta.ActualizarAsync(cuenta);
            await _repositorioCuenta.GuardarAsync();

            
            var cuentaActualizadaDto = new CuentaDto
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta.ToString(),
                SaldoInicial = cuenta.SaldoInicial,
                Estado = cuenta.Estado,
                ClienteId = cuenta.ClienteId,
                Cliente = await ObtenerNombreClientePorId(cuenta.ClienteId) 
            };

            return cuentaActualizadaDto;
        }


        private void ActualizarCamposCuenta(Cuenta cuenta, ActualizarCuentaParametros parametros)
        {
            // Solo actualizar los campos que no son null
            if (parametros.NumeroCuenta.HasValue)
            {
                cuenta.NumeroCuenta = parametros.NumeroCuenta.Value;
            }

            if (parametros.TipoCuenta.HasValue)
            {
                cuenta.TipoCuenta = parametros.TipoCuenta.Value;
            }

            if (parametros.SaldoInicial.HasValue)
            {
                cuenta.SaldoInicial = parametros.SaldoInicial.Value;
            }

            if (parametros.Estado.HasValue)
            {
                cuenta.Estado = parametros.Estado.Value;
            }

            if (parametros.ClienteId.HasValue)
            {
                cuenta.ClienteId = parametros.ClienteId.Value;
            }
        }

        public async Task EliminarCuentaAsync(int numeroCuenta)
        {
            var cuenta = await _repositorioCuenta.BuscarAsync(c => c.NumeroCuenta == numeroCuenta);
            var cuentaEliminar = cuenta.FirstOrDefault();

            if (cuentaEliminar == null)
            {
                throw new Exception("La cuenta no existe.");
            }

            await _repositorioCuenta.EliminarAsync(cuentaEliminar);
            await _repositorioCuenta.GuardarAsync();
        }




    }
}
