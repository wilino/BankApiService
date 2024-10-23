using Moq;
using ServicioClientes.Aplicacion.Servicios.Imp;
using ServicioClientes.Aplicacion.Parametros;
using ServicioClientes.Dominio.Entidades;
using ServicioClientes.Infraestructura.Repositorios;
using Microsoft.Extensions.Configuration;

namespace ServicioClientes.Pruebas.PruebasIntegracion
{
    public class ClienteServicioPruebas
    {
        private readonly Mock<IRepositorioGenerico<Cliente>> _repositorioClienteMock;
        private readonly Mock<IRepositorioGenerico<Persona>> _repositorioPersonaMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly ClienteServicio _clienteServicio;

        public ClienteServicioPruebas()
        {
            // Inicializamos los mocks
            _repositorioClienteMock = new Mock<IRepositorioGenerico<Cliente>>();
            _repositorioPersonaMock = new Mock<IRepositorioGenerico<Persona>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();

            // Creamos un cliente HTTP mockeado
            var httpClient = new HttpClient();
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Creamos el servicio con los mocks
            _clienteServicio = new ClienteServicio(
                _repositorioClienteMock.Object,
                _repositorioPersonaMock.Object,
                httpClient,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task CrearClienteAsync_DeberiaCrearClienteNuevo_SiNoExiste()
        {
            // Arrange
            var parametros = new CrearClienteParametros
            {
                Nombre = "Juan",
                Direccion = "123 Calle",
                Telefono = "555-1234",
                Contrasena = "password123",
                Estado = true
            };

            _repositorioClienteMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Cliente, bool>>>()))
                .ReturnsAsync(new List<Cliente>());
            _repositorioPersonaMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Persona, bool>>>()))
                .ReturnsAsync(new List<Persona>());

            _repositorioClienteMock.Setup(r => r.InsertarAsync(It.IsAny<Cliente>())).Returns(Task.CompletedTask);
            _repositorioClienteMock.Setup(r => r.GuardarAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _clienteServicio.CrearClienteAsync(parametros);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Juan", result.Nombre);
            Assert.Equal("123 Calle", result.Direccion);
            Assert.Equal("555-1234", result.Telefono);
        }

        [Fact]
        public async Task CrearClienteAsync_DeberiaLanzarExcepcion_SiElClienteYaExiste()
        {
            // Arrange
            var parametros = new CrearClienteParametros
            {
                Nombre = "Juan",
                Direccion = "123 Calle",
                Telefono = "555-1234",
                Contrasena = "password123",
                Estado = true
            };

            var clienteExistente = new List<Cliente> { new Cliente { ClienteId = 1, PersonaId = 1 } };

            // Simulamos que el cliente ya existe
            _repositorioClienteMock.Setup(r => r.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Cliente, bool>>>()))
                .ReturnsAsync(clienteExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<System.Exception>(async () =>
                await _clienteServicio.CrearClienteAsync(parametros));

            Assert.Equal("El cliente ya está registrado.", exception.Message);
        }
    }
}
