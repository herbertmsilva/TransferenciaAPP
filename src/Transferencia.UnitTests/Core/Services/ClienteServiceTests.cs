using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using Transferencia.Application.Dtos.Cliente;
using Transferencia.Application.Exceptions;
using Transferencia.Application.Interfaces.Mappers;
using Transferencia.Application.Interfaces.Services;
using Transferencia.Application.Services;
using Transferencia.Core.Entities;
using Transferencia.Core.Interfaces.Persistence.Repositories;

namespace Transferencia.Tests.Services
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly Mock<IValidator<CreateClienteDTO>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateClienteDTO>> _updateValidatorMock;
        private readonly Mock<IClienteMapper> _clienteMapperMock;
        private readonly IClienteService _clienteService;

        public ClienteServiceTests()
        {
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _createValidatorMock = new Mock<IValidator<CreateClienteDTO>>();
            _updateValidatorMock = new Mock<IValidator<UpdateClienteDTO>>();
            _clienteMapperMock = new Mock<IClienteMapper>();

            _clienteService = new ClienteService(
                _clienteRepositoryMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _clienteMapperMock.Object
            );
        }

        [Fact]
        public async Task GetAllClientesAsync_ShouldReturnListOfClientes()
        {
            // Arrange
            var clientes = new List<ClienteEntity>
            {
                new ClienteEntity { Id = Guid.NewGuid(), Nome = "Cliente 1", NumeroConta = "12345", Saldo = 1000 },
                new ClienteEntity { Id = Guid.NewGuid(), Nome = "Cliente 2", NumeroConta = "67890", Saldo = 2000 }
            };

            _clienteRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(clientes);
            _clienteMapperMock.Setup(mapper => mapper.ConvertToDtoForRead(It.IsAny<ClienteEntity>()))
                              .Returns((ClienteEntity c) => new ClienteDTO
                              {
                                  Id = c.Id,
                                  Nome = c.Nome,
                                  NumeroConta = c.NumeroConta,
                                  Saldo = c.Saldo
                              });

            // Act
            var result = await _clienteService.GetAllClientesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Nome.Should().Be("Cliente 1");
            result.Last().Nome.Should().Be("Cliente 2");
        }

        [Fact]
        public async Task GetClienteByIdAsync_ShouldReturnCliente_WhenClienteExists()
        {
            // Arrange
            var cliente = new ClienteEntity { Id = Guid.NewGuid(), Nome = "Cliente 1", NumeroConta = "12345", Saldo = 1000 };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
            _clienteMapperMock.Setup(mapper => mapper.ConvertToDtoForRead(cliente))
                              .Returns(new ClienteDTO
                              {
                                  Id = cliente.Id,
                                  Nome = cliente.Nome,
                                  NumeroConta = cliente.NumeroConta,
                                  Saldo = cliente.Saldo
                              });

            // Act
            var result = await _clienteService.GetClienteByIdAsync(cliente.Id);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Cliente 1");
        }

        [Fact]
        public async Task GetClienteByIdAsync_ShouldThrowCustomException_WhenClienteDoesNotExist()
        {
            // Arrange
            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ClienteEntity)null);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _clienteService.GetClienteByIdAsync(Guid.NewGuid()));

            exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            exception.Message.Should().Be("Cliente não encontrado.");
        }

        [Fact]
        public async Task GetClienteByIdAsync_ShouldThrowCustomException_WhenIdNotSend()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _clienteService.GetClienteByIdAsync(Guid.Empty));

            exception.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            exception.Message.Should().Be("ID inválido.");
        }

        [Fact]
        public async Task AddClienteAsync_ShouldAddCliente_WhenValidationSucceeds()
        {
            // Arrange
            var createDto = new CreateClienteDTO { Nome = "Cliente 1", NumeroConta = "12345", Saldo = 1000 };
            var clienteEntity = new ClienteEntity { Id = Guid.NewGuid(), Nome = "Cliente 1", NumeroConta = "12345", Saldo = 1000 };

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _clienteMapperMock.Setup(mapper => mapper.ConvertToEntityForCreate(createDto)).Returns(clienteEntity);
            _clienteRepositoryMock.Setup(repo => repo.AddAsync(clienteEntity)).Returns(Task.CompletedTask);
            _clienteRepositoryMock.Setup(repo => repo.SaveAsync()).ReturnsAsync(1);
            _clienteMapperMock.Setup(mapper => mapper.ConvertToDtoForRead(clienteEntity))
                              .Returns(new ClienteDTO { Id = clienteEntity.Id, Nome = clienteEntity.Nome, NumeroConta = clienteEntity.NumeroConta, Saldo = clienteEntity.Saldo });

            // Act
            var result = await _clienteService.AddClienteAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Cliente 1");
            _clienteRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ClienteEntity>()), Times.Once);
            _clienteRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task AddClienteAsync_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var createDto = new CreateClienteDTO { Nome = "Cliente 1", NumeroConta = "12345", Saldo = 1000 };
            var validationFailures = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Nome", "O nome é obrigatório.")
            };

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));

            // Act
            Func<Task> act = async () => await _clienteService.AddClienteAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ValidationException>();
            exception.WithMessage("*O nome é obrigatório*");
        }

        [Fact]
        public async Task UpdateClienteAsync_ShouldUpdateCliente_WhenValidationSucceeds()
        {
            // Arrange
            var updateDto = new UpdateClienteDTO { Nome = "Cliente Atualizado", NumeroConta = "54321", Saldo = 2000 };
            var clienteEntity = new ClienteEntity { Id = Guid.NewGuid(), Nome = "Cliente 1", NumeroConta = "12345", Saldo = 1000 };

            _updateValidatorMock.Setup(validator => validator.ValidateAsync(updateDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteEntity.Id)).ReturnsAsync(clienteEntity);
            _clienteRepositoryMock.Setup(repo => repo.UpdateAsync(clienteEntity)).Returns(Task.CompletedTask);
            _clienteRepositoryMock.Setup(repo => repo.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _clienteService.UpdateClienteAsync(clienteEntity.Id, updateDto);

            // Assert
            result.Should().BeTrue();
            _clienteRepositoryMock.Verify(repo => repo.UpdateAsync(clienteEntity), Times.Once);
            _clienteRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateClienteAsync_ShouldReturnFalse_WhenClienteDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateClienteDTO { Nome = "Cliente Atualizado", NumeroConta = "54321", Saldo = 2000 };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ClienteEntity)null);

            _updateValidatorMock.Setup(validator => validator.ValidateAsync(updateDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _clienteService.UpdateClienteAsync(Guid.NewGuid(), updateDto);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteClienteAsync_ShouldReturnTrue_WhenClienteExists()
        {
            // Arrange
            var clienteEntity = new ClienteEntity { Id = Guid.NewGuid(), Nome = "Cliente 1", NumeroConta = "12345", Saldo = 1000 };

            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(clienteEntity.Id)).ReturnsAsync(clienteEntity);
            _clienteRepositoryMock.Setup(repo => repo.DeleteAsync(clienteEntity)).Returns(Task.CompletedTask);
            _clienteRepositoryMock.Setup(repo => repo.SaveAsync()).ReturnsAsync(1);

            // Act
            var result = await _clienteService.DeleteClienteAsync(clienteEntity.Id);

            // Assert
            result.Should().BeTrue();
            _clienteRepositoryMock.Verify(repo => repo.DeleteAsync(clienteEntity), Times.Once);
            _clienteRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteClienteAsync_ShouldThrowCustomException_WhenClienteDoesNotExist()
        {
            // Arrange
            _clienteRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ClienteEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _clienteService.DeleteClienteAsync(Guid.NewGuid()));

            exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            exception.Message.Should().Be("Cliente não encontrado.");
        }

        [Fact]
        public async Task DeleteClienteAsync_ShouldThrowCustomException_WhenIdNotSend()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _clienteService.DeleteClienteAsync(Guid.Empty));

            exception.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            exception.Message.Should().Be("ID inválido.");
        }

        [Fact]
        public async Task AddClienteAsync_ShouldThrowException_WhenAccountNumberAlreadyExists()
        {
            // Arrange
            var createClienteDto = new CreateClienteDTO
            {
                Nome = "Cliente Teste",
                NumeroConta = "123456",
                Saldo = 1000
            };

            var existingCliente = new ClienteEntity { Id = Guid.NewGuid(), Nome = "Cliente Existente", NumeroConta = "123456", Saldo = 2000 };

            _clienteRepositoryMock.Setup(repo => repo.GetByNumeroContaAsync(createClienteDto.NumeroConta))
                                  .ReturnsAsync(existingCliente);

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createClienteDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            Func<Task> act = async () => await _clienteService.AddClienteAsync(createClienteDto);

            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("O número da conta já está em uso.");

            _clienteRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<ClienteEntity>()), Times.Never);
        }

    }
}
