using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ServicioCuentas.API;
using ServicioClientes.Aplicacion.DTOs;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ServicioClientes.Pruebas.PruebasIntegracion
{
    public class ServicioCuentasIntegracionPruebas : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public ServicioCuentasIntegracionPruebas(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task ObtenerCuentaPorNumero_DeberiaRetornarCuentaCorrectamente()
        {
            // Arrange
            string numeroCuenta = "123456";

            // Act
            var response = await _client.GetAsync($"/api/Cuentas/obtener-cuenta?numeroCuenta={numeroCuenta}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var cuentaDto = JsonConvert.DeserializeObject<CuentaDto>(content);

            Assert.NotNull(cuentaDto);
            Assert.Equal(123456, cuentaDto.NumeroCuenta);
        }
    }
}