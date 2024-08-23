using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Transferencia.Application.Dtos.Cliente;
using Transferencia.Application.Interfaces;

namespace Transferencia.IntegrationTests
{
    public class ClienteControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ClienteControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllClientes_ShouldReturnOk_WhenClientesExist()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/cliente");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var clientes = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<ClienteDTO>>>();
            clientes.Should().NotBeNull();
        }

        [Fact]
        public async Task GetClienteById_ShouldReturnNotFound_WhenClienteDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/cliente/{Guid.NewGuid()}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddCliente_ShouldReturnCreated_WhenDataIsValid()
        {
            // Arrange
            var cliente = new CreateClienteDTO
            {
                Nome = "Cliente Teste",
                NumeroConta = "123456",
                Saldo = 1000
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/cliente", cliente);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdCliente = await response.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();
            createdCliente?.Data.Should().NotBeNull();
            createdCliente?.Data.Nome.Should().Be("Cliente Teste");
        }

        [Fact]
        public async Task AddCliente_Then_GetClienteById_ShouldReturnCorrectCliente()
        {
            // Arrange
            var newCliente = new CreateClienteDTO
            {
                Nome = "Cliente Teste",
                NumeroConta = "654321",
                Saldo = 5000
            };

            // Act
            var postResponse = await _client.PostAsJsonAsync("/api/v1/cliente", newCliente);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdCliente = await postResponse.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();

            // Act
            var getResponse = await _client.GetAsync($"/api/v1/cliente/{createdCliente?.Data.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetchedCliente = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();

            // Assert
            fetchedCliente?.Data.Should().NotBeNull();
            fetchedCliente?.Data.Nome.Should().Be(newCliente.Nome);
            fetchedCliente?.Data.NumeroConta.Should().Be(newCliente.NumeroConta);
            fetchedCliente?.Data.Saldo.Should().Be(newCliente.Saldo);
        }

        [Fact]
        public async Task UpdateCliente_ShouldUpdateClienteDetails()
        {
            // Arrange
            var cliente = new CreateClienteDTO
            {
                Nome = "Cliente Original",
                NumeroConta = "789123",
                Saldo = 2000
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/cliente", cliente);
            var createdCliente = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();

            // Act
            var updatedCliente = new UpdateClienteDTO
            {
                Id = createdCliente.Data.Id,
                Nome = "Cliente Atualizado",
                NumeroConta = "789123",
                Saldo = 3000
            };
            var updateResponse = await _client.PutAsJsonAsync($"/api/v1/cliente/{createdCliente.Data.Id}", updatedCliente);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
            var getResponse = await _client.GetAsync($"/api/v1/cliente/{createdCliente.Data.Id}");
            var fetchedCliente = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();
            fetchedCliente?.Data.Should().NotBeNull();
            fetchedCliente?.Data.Nome.Should().Be("Cliente Atualizado");
            fetchedCliente?.Data.Saldo.Should().Be(3000);
        }

        [Fact]
        public async Task DeleteCliente_ShouldRemoveCliente()
        {
            // Arrange
            var cliente = new CreateClienteDTO
            {
                Nome = "Cliente a Ser Deletado",
                NumeroConta = "456789",
                Saldo = 1000
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/cliente", cliente);
            var createdCliente = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/v1/cliente/{createdCliente.Data.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
            var getResponse = await _client.GetAsync($"/api/v1/cliente/{createdCliente.Data.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task GetClienteByNumeroConta_ShouldReturnClienteDetails()
        {
            // Arrange
            var cliente = new CreateClienteDTO
            {
                Nome = "Cliente por Conta",
                NumeroConta = "112233",
                Saldo = 1500
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/cliente", cliente);
            var createdCliente = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();

            // Act 
            var getResponse = await _client.GetAsync($"/api/v1/cliente/por-conta/{createdCliente.Data.NumeroConta}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetchedCliente = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ClienteDTO>>();

            // Assert
            fetchedCliente?.Data.Should().NotBeNull();
            fetchedCliente?.Data.Id.Should().Be(createdCliente.Data.Id);
            fetchedCliente?.Data.NumeroConta.Should().Be("112233");
        }

        [Fact]
        public async Task AddCliente_ShouldReturnBadRequest_WhenNomeIsEmpty()
        {
            var invalidCliente = new CreateClienteDTO
            {
                Nome = "",
                NumeroConta = "123456",
                Saldo = 1000
            };

            // Act 
            var response = await _client.PostAsJsonAsync("/api/v1/cliente", invalidCliente);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("O nome do cliente é obrigatório"); 
        }

        [Fact]
        public async Task AddCliente_ShouldReturnBadRequest_WhenSaldoIsNegative()
        {
            // Arrange
            var invalidCliente = new CreateClienteDTO
            {
                Nome = "Cliente Teste",
                NumeroConta = "123456", 
                Saldo = -500 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/cliente", invalidCliente);

            // Assert 
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("O saldo do cliente não pode ser negativo");
        }

    }
}