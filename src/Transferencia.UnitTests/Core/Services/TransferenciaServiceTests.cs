using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Transferencia.Application.Dtos.Transferencia;
using Transferencia.Application.Exceptions;
using Transferencia.Application.Interfaces.Mappers;
using Transferencia.Application.Services;
using Transferencia.Core.Entities;
using Transferencia.Core.Enums;
using Transferencia.Core.Interfaces.Persistence.Repositories;

namespace Transferencia.Tests.Services
{
    public class TransferenciaServiceTests
    {
        private readonly Mock<ITransferenciaRepository> _transferenciaRepositoryMock;
        private readonly Mock<IValidator<CreateTransferenciaDTO>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateTransferenciaDTO>> _updateValidatorMock;
        private readonly Mock<ITransferenciaMapper> _transferenciaMapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TransferenciaService _transferenciaService;
        private readonly Mock<IDbContextTransaction> _mockTransaction;

        public TransferenciaServiceTests()
        {
            _transferenciaRepositoryMock = new Mock<ITransferenciaRepository>();
            _createValidatorMock = new Mock<IValidator<CreateTransferenciaDTO>>();
            _updateValidatorMock = new Mock<IValidator<UpdateTransferenciaDTO>>();
            _transferenciaMapperMock = new Mock<ITransferenciaMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mockTransaction = new Mock<IDbContextTransaction>();

            _transferenciaService = new TransferenciaService(
                _transferenciaRepositoryMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _transferenciaMapperMock.Object,
                _unitOfWorkMock.Object
            );


        }

        [Fact]
        public async Task GetAllTransferenciasAsync_ShouldReturnListOfTransferencias()
        {
            // Arrange
            var transferencias = new List<TransferenciaEntity>
            {
                new TransferenciaEntity { Id = Guid.NewGuid(), Valor = 1000, Status = StatusTransferenciaEnum.Sucesso },
                new TransferenciaEntity { Id = Guid.NewGuid(), Valor = 2000, Status = StatusTransferenciaEnum.Sucesso }
            };

            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetAllAsync()).ReturnsAsync(transferencias);
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToDtoForRead(It.IsAny<TransferenciaEntity>()))
                                    .Returns((TransferenciaEntity t) => new TransferenciaDTO
                                    {
                                        Id = t.Id,
                                        Valor = t.Valor,
                                        Status = t.Status
                                    });

            // Act
            var result = await _transferenciaService.GetAllTransferenciasAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Valor.Should().Be(1000);
            result.Last().Valor.Should().Be(2000);
        }

        [Fact]
        public async Task GetTransferenciaByIdAsync_ShouldReturnTransferencia_WhenTransferenciaExists()
        {
            // Arrange
            var transferencia = new TransferenciaEntity { Id = Guid.NewGuid(), Valor = 1000, Status = StatusTransferenciaEnum.Sucesso };

            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetByIdAsync(transferencia.Id)).ReturnsAsync(transferencia);
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToDtoForRead(transferencia))
                                    .Returns(new TransferenciaDTO
                                    {
                                        Id = transferencia.Id,
                                        Valor = transferencia.Valor,
                                        Status = transferencia.Status
                                    });

            // Act
            var result = await _transferenciaService.GetTransferenciaByIdAsync(transferencia.Id);

            // Assert
            result.Should().NotBeNull();
            result.Valor.Should().Be(1000);
        }

        [Fact]
        public async Task GetTransferenciaByIdAsync_ShouldReturnNull_WhenTransferenciaDoesNotExist()
        {
            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TransferenciaEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _transferenciaService.CancelarTransferenciaAsync(Guid.NewGuid()));

            exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            exception.Message.Should().Be("Transferencia não encontrada.");
        }

        [Fact]
        public async Task RealizarTransferenciaAsync_ShouldThrowCustomException_WhenContaOrigemDoesNotExist()
        {
            // Arrange
            var createDto = new CreateTransferenciaDTO { ContaOrigemId = Guid.NewGuid(), ContaDestinoId = Guid.NewGuid(), Valor = 1000 };

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            
            _mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaOrigemId)).ReturnsAsync((ClienteEntity)null);
            _unitOfWorkMock.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);


            // Act
            Func<Task> act = async () => await _transferenciaService.RealizarTransferenciaAsync(createDto);

            var transferenciaFalhaEntity = new TransferenciaEntity { Status = StatusTransferenciaEnum.Falha };
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToEntityForCreate(createDto))
                                    .Returns(transferenciaFalhaEntity);

            _unitOfWorkMock.Setup(uow => uow.Transferencias.AddAsync(transferenciaFalhaEntity)).Returns(Task.CompletedTask);


            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("Conta de origem não encontrada.");
        }

        [Fact]
        public async Task RealizarTransferenciaAsync_ShouldThrowCustomException_WhenContaDestinoDoesNotExist()
        {
            // Arrange
            var contaOrigem = new ClienteEntity { Id = Guid.NewGuid(), Saldo = 2000 };
            var createDto = new CreateTransferenciaDTO { ContaOrigemId = contaOrigem.Id, ContaDestinoId = Guid.NewGuid(), Valor = 1000 };

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaOrigemId)).ReturnsAsync(contaOrigem);
            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaDestinoId)).ReturnsAsync((ClienteEntity)null);
            _unitOfWorkMock.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);


            // Act
            Func<Task> act = async () => await _transferenciaService.RealizarTransferenciaAsync(createDto);


            var transferenciaFalhaEntity = new TransferenciaEntity { Status = StatusTransferenciaEnum.Falha };
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToEntityForCreate(createDto))
                                    .Returns(transferenciaFalhaEntity); 

            _unitOfWorkMock.Setup(uow => uow.Transferencias.AddAsync(transferenciaFalhaEntity)).Returns(Task.CompletedTask);


            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("Conta de destino não encontrada.");
        }

        [Fact]
        public async Task RealizarTransferenciaAsync_ShouldThrowCustomException_WhenSaldoInsuficiente()
        {
            // Arrange
            var contaOrigem = new ClienteEntity { Id = Guid.NewGuid(), Saldo = 500 };
            var contaDestino = new ClienteEntity { Id = Guid.NewGuid() };
            var createDto = new CreateTransferenciaDTO { ContaOrigemId = contaOrigem.Id, ContaDestinoId = contaDestino.Id, Valor = 1000 };

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaOrigemId)).ReturnsAsync(contaOrigem);
            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaDestinoId)).ReturnsAsync(contaDestino);
            _unitOfWorkMock.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);

            // Act
            Func<Task> act = async () => await _transferenciaService.RealizarTransferenciaAsync(createDto);

            var transferenciaFalhaEntity = new TransferenciaEntity { Status = StatusTransferenciaEnum.Falha };
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToEntityForCreate(createDto))
                                    .Returns(transferenciaFalhaEntity);

            _unitOfWorkMock.Setup(uow => uow.Transferencias.AddAsync(transferenciaFalhaEntity)).Returns(Task.CompletedTask);



            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("Saldo insuficiente na conta de origem.");
        }

        [Fact]
        public async Task RealizarTransferenciaAsync_ShouldCompleteTransferencia_WhenValidData()
        {
            // Arrange
            var contaOrigem = new ClienteEntity { Id = Guid.NewGuid(), Saldo = 2000 };
            var contaDestino = new ClienteEntity { Id = Guid.NewGuid() };
            var createDto = new CreateTransferenciaDTO { ContaOrigemId = contaOrigem.Id, ContaDestinoId = contaDestino.Id, Valor = 1000 };
            var transferenciaEntity = new TransferenciaEntity { Id = Guid.NewGuid(), Valor = 1000, Status = StatusTransferenciaEnum.Sucesso };

            _mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaOrigemId)).ReturnsAsync(contaOrigem);
            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaDestinoId)).ReturnsAsync(contaDestino);
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToEntityForCreate(createDto)).Returns(transferenciaEntity);
            _unitOfWorkMock.Setup(uow => uow.Transferencias.AddAsync(transferenciaEntity)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);

            
            _unitOfWorkMock.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);

            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToDtoForRead(transferenciaEntity))
                                    .Returns(new TransferenciaDTO
                                    {
                                        Id = transferenciaEntity.Id,
                                        Valor = transferenciaEntity.Valor,
                                        Status = transferenciaEntity.Status
                                    });

            // Act
            var result = await _transferenciaService.RealizarTransferenciaAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Valor.Should().Be(1000);
            result.Status.Should().Be(StatusTransferenciaEnum.Sucesso);
            _unitOfWorkMock.Verify(uow => uow.Transferencias.AddAsync(transferenciaEntity), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelarTransferenciaAsync_ShouldReturnTrue_WhenTransferenciaExists()
        {
            // Arrange
            var transferencia = new TransferenciaEntity { Id = Guid.NewGuid(), Status = StatusTransferenciaEnum.Sucesso };

            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetByIdAsync(transferencia.Id)).ReturnsAsync(transferencia);
            _unitOfWorkMock.Setup(uow => uow.Transferencias.UpdateAsync(transferencia)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _transferenciaService.CancelarTransferenciaAsync(transferencia.Id);

            // Assert
            result.Should().BeTrue();
            transferencia.Status.Should().Be(StatusTransferenciaEnum.Cancelada);
            _unitOfWorkMock.Verify(uow => uow.Transferencias.UpdateAsync(transferencia), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelarTransferenciaAsync_ShouldReturnFalse_WhenTransferenciaIdNotSend()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _transferenciaService.CancelarTransferenciaAsync(Guid.Empty));

            exception.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            exception.Message.Should().Be("ID inválido.");
        }

        [Fact]
        public async Task CancelarTransferenciaAsync_ShouldReturnFalse_WhenTransferenciaDoesNotExist()
        {
            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TransferenciaEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _transferenciaService.CancelarTransferenciaAsync(Guid.NewGuid()));

            exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            exception.Message.Should().Be("Transferencia não encontrada.");
        }

        [Fact]
        public async Task DeleteTransferenciaAsync_ShouldReturnTrue_WhenTransferenciaExists()
        {
            // Arrange
            var transferencia = new TransferenciaEntity { Id = Guid.NewGuid() };

            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetByIdAsync(transferencia.Id)).ReturnsAsync(transferencia);
            _unitOfWorkMock.Setup(uow => uow.Transferencias.DeleteAsync(transferencia)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _transferenciaService.DeleteTransferenciaAsync(transferencia.Id);

            // Assert
            result.Should().BeTrue();
            _unitOfWorkMock.Verify(uow => uow.Transferencias.DeleteAsync(transferencia), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTransferenciaAsync_ShouldReturnFalse_WhenTransferenciaDoesNotExist()
        {
            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TransferenciaEntity)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _transferenciaService.CancelarTransferenciaAsync(Guid.NewGuid()));

            exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            exception.Message.Should().Be("Transferencia não encontrada.");
        }

        [Fact]
        public async Task GetHistoricoTransferenciasAsync_ShouldReturnListOfTransferencias()
        {
            // Arrange
            var transferencias = new List<TransferenciaEntity>
            {
                new TransferenciaEntity { Id = Guid.NewGuid(), Valor = 1000, Status = StatusTransferenciaEnum.Sucesso },
                new TransferenciaEntity { Id = Guid.NewGuid(), Valor = 2000, Status = StatusTransferenciaEnum.Sucesso }
            };

            _unitOfWorkMock.Setup(uow => uow.Transferencias.GetHistoricoPorContaAsync(It.IsAny<Guid>())).ReturnsAsync(transferencias);
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToDtoForRead(It.IsAny<TransferenciaEntity>()))
                                    .Returns((TransferenciaEntity t) => new TransferenciaDTO
                                    {
                                        Id = t.Id,
                                        Valor = t.Valor,
                                        Status = t.Status
                                    });

            // Act
            var result = await _transferenciaService.GetHistoricoTransferenciasAsync(Guid.NewGuid());

            // Assert
            result.Should().HaveCount(2);
            result.First().Valor.Should().Be(1000);
            result.Last().Valor.Should().Be(2000);
        }

        [Fact]
        public async Task RealizarTransferenciaAsync_ShouldThrowException_WhenAmountExceedsLimit()
        {
            // Arrange
            var contaOrigem = new ClienteEntity { Id = Guid.NewGuid(), Saldo = 15000 };
            var contaDestino = new ClienteEntity { Id = Guid.NewGuid() };
            var createDto = new CreateTransferenciaDTO { ContaOrigemId = contaOrigem.Id, ContaDestinoId = contaDestino.Id, Valor = 15000 }; 

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaOrigemId)).ReturnsAsync(contaOrigem);
            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaDestinoId)).ReturnsAsync(contaDestino);
            _unitOfWorkMock.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);

            // Act
            Func<Task> act = async () => await _transferenciaService.RealizarTransferenciaAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("O valor da transferência não pode exceder R$ 10.000,00.");
        }

        [Fact]
        public async Task RealizarTransferenciaAsync_ShouldRegisterFailureInHistory_WhenInsufficientBalance()
        {
            // Arrange
            var contaOrigem = new ClienteEntity { Id = Guid.NewGuid(), Saldo = 500 };
            var contaDestino = new ClienteEntity { Id = Guid.NewGuid() };
            var createDto = new CreateTransferenciaDTO { ContaOrigemId = contaOrigem.Id, ContaDestinoId = contaDestino.Id, Valor = 1000 };

            _createValidatorMock.Setup(validator => validator.ValidateAsync(createDto, default))
                                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaOrigemId)).ReturnsAsync(contaOrigem);
            _unitOfWorkMock.Setup(uow => uow.Clientes.GetByIdAsync(createDto.ContaDestinoId)).ReturnsAsync(contaDestino);
            _unitOfWorkMock.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);

            
            var transferenciaFalhaEntity = new TransferenciaEntity { Status = StatusTransferenciaEnum.Falha };
            _transferenciaMapperMock.Setup(mapper => mapper.ConvertToEntityForCreate(createDto))
                                    .Returns(transferenciaFalhaEntity);

            _unitOfWorkMock.Setup(uow => uow.Transferencias.AddAsync(transferenciaFalhaEntity)).Returns(Task.CompletedTask);


            // Act
            Func<Task> act = async () => await _transferenciaService.RealizarTransferenciaAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<CustomException>()
                .WithMessage("Saldo insuficiente na conta de origem.");

            _unitOfWorkMock.Verify(uow => uow.Transferencias.AddAsync(It.Is<TransferenciaEntity>(t => t.Status == StatusTransferenciaEnum.Falha)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

    }
}
