using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using Transferencia.Application.Dtos.Transferencia;
using Transferencia.Application.Interfaces;
using Transferencia.IntegrationTests;
using Transferencia.Persistence.Data;

namespace Transferencia.IntegrationTests
{
    public class TransferenciaControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public TransferenciaControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllTransferencias_ShouldReturnOk_WhenTransferenciasExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1.0/Transferencia");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var transferencias = JsonSerializer.Deserialize<ApiResponse<IEnumerable<TransferenciaDTO>>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(transferencias?.Data);
            Assert.True(transferencias?.Data.Any());
        }

        [Fact]
        public async Task GetTransferenciaById_ShouldReturnOk_WhenTransferenciaExists()
        {
            // Arrange

            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<AppDbContext>();

            var transferenciaId = dbContext.Transferencias.First().Id;

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/api/v1.0/Transferencia/{transferenciaId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var transferencia = JsonSerializer.Deserialize<ApiResponse<TransferenciaDTO>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(transferencia?.Data);
            Assert.Equal(transferenciaId, transferencia?.Data.Id);
        }

        [Fact]
        public async Task RealizarTransferencia_ShouldCreateTransferencia_WhenDataIsValid()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<AppDbContext>();

                var clienteOrigem = dbContext.Clientes.First();
                var clienteDestino = dbContext.Clientes.Last();

                var createTransferenciaDto = new CreateTransferenciaDTO
                {
                    ContaOrigemId = clienteDestino.Id,
                    ContaDestinoId = clienteOrigem.Id,
                    Valor = 500,
                    DataTransferencia = DateTime.Now.AddDays(10)
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(createTransferenciaDto), Encoding.UTF8, "application/json");

                // Act
                var response = await _client.PostAsync("/api/v1.0/Transferencia", jsonContent);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
        }

        [Fact]
        public async Task RealizarTransferenciasConcorrentes_DeveManterIntegridadeDoSaldo()
        {
            // Arrange
            using (var scope1 = _factory.Services.CreateScope())
            using (var scope2 = _factory.Services.CreateScope())
            {
                var dbContext1 = scope1.ServiceProvider.GetRequiredService<AppDbContext>();
                var dbContext2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();

                var clienteOrigemId = dbContext1.Clientes.First().Id;
                var clienteDestinoId = dbContext1.Clientes.Last().Id;

                var saldoInicialOrigem = dbContext1.Clientes.First().Saldo;

                var transferenciaDto1 = new CreateTransferenciaDTO
                {
                    ContaOrigemId = clienteOrigemId,
                    ContaDestinoId = clienteDestinoId,
                    Valor = 300,
                    DataTransferencia = DateTime.Now.AddDays(1)
                };

                var transferenciaDto2 = new CreateTransferenciaDTO
                {
                    ContaOrigemId = clienteOrigemId,
                    ContaDestinoId = clienteDestinoId,
                    Valor = 700,
                    DataTransferencia = DateTime.Now.AddDays(1)
                };

                var jsonContent1 = new StringContent(JsonSerializer.Serialize(transferenciaDto1), Encoding.UTF8, "application/json");
                var jsonContent2 = new StringContent(JsonSerializer.Serialize(transferenciaDto2), Encoding.UTF8, "application/json");

                // Act
                var client1 = _factory.CreateClient();
                var client2 = _factory.CreateClient();

                var task1 = client1.PostAsync("/api/v1.0/Transferencia", jsonContent1);
                var task2 = client2.PostAsync("/api/v1.0/Transferencia", jsonContent2);

                var response1 = await task1;
                var response2 = await task2;

                var dbContextFinal = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
                var saldoFinalOrigem = dbContextFinal.Clientes.First(c => c.Id == clienteOrigemId).Saldo;

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    Assert.Equal(saldoInicialOrigem - 1000, saldoFinalOrigem);
                }
            }
        }

        [Fact]
        public async Task CancelarTransferencia_ShouldReturnOk_WhenTransferenciaIsCancelled()
        {
            // Arrange

            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<AppDbContext>();

            var transferenciaId = dbContext.Transferencias.First().Id;
            var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsync($"/api/v1.0/Transferencia/cancelar/{transferenciaId}", null);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(resultado);
            Assert.True(resultado.Success);
        }


        [Fact]
        public async Task DeleteTransferencia_ShouldReturnOk_WhenTransferenciaIsDeleted()
        {
            // Arrange

            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<AppDbContext>();

            var transferenciaId = dbContext.Transferencias.First().Id;
            var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/v1.0/Transferencia/{transferenciaId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(resultado);
            Assert.True(resultado.Success);
        }

        [Fact]
        public async Task GetHistoricoTransferencias_ShouldReturnOk_WhenHistoricoExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<AppDbContext>();

            var contaId = dbContext.Clientes.Last().Id;

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/api/v1.0/Transferencia/historico/{contaId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var historico = JsonSerializer.Deserialize<ApiResponse<IEnumerable<TransferenciaDTO>>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(historico?.Data);
            Assert.True(historico?.Data.Any());
        }
    }
}