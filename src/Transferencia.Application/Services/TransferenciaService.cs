using Transferencia.Core.Enums;
using Transferencia.Core.Interfaces.Persistence.Repositories;
using FluentValidation;
using Transferencia.Application.Interfaces.Services;
using Transferencia.Application.Dtos.Transferencia;
using Transferencia.Application.Interfaces.Mappers;
using Transferencia.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Transferencia.Core.Entities;

namespace Transferencia.Application.Services
{
    public class TransferenciaService : ITransferenciaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransferenciaRepository _transferenciaRepository;
        private readonly IValidator<CreateTransferenciaDTO> _createValidator;
        private readonly IValidator<UpdateTransferenciaDTO> _updateValidator;
        private readonly ITransferenciaMapper _transferenciaMapper;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public TransferenciaService(
            ITransferenciaRepository transferenciaRepository,
            IValidator<CreateTransferenciaDTO> createValidator,
            IValidator<UpdateTransferenciaDTO> updateValidator,
            ITransferenciaMapper transferenciaMapper,
            IUnitOfWork unitOfWork)
        {
            _transferenciaRepository = transferenciaRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _transferenciaMapper = transferenciaMapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TransferenciaDTO>> GetAllTransferenciasAsync()
        {
            var transferencias = await _unitOfWork.Transferencias.GetAllAsync();
            return transferencias.Select(t => _transferenciaMapper.ConvertToDtoForRead(t));
        }

        public async Task<TransferenciaDTO> GetTransferenciaByIdAsync(Guid id)
        {
            var transferencia = await GetTranferenciaById(id);
            return _transferenciaMapper.ConvertToDtoForRead(transferencia);
        }

        public async Task<TransferenciaDTO> RealizarTransferenciaAsync(CreateTransferenciaDTO createTransferenciaDTO)
        {
            
            var validationResult = await _createValidator.ValidateAsync(createTransferenciaDTO);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (createTransferenciaDTO.Valor > 10000)
            {
                throw new CustomException("O valor da transferência não pode exceder R$ 10.000,00.", 400);
            }

            await _semaphore.WaitAsync();

            try
            {
                var contaOrigem = await _unitOfWork.Clientes.GetByIdAsync(createTransferenciaDTO.ContaOrigemId);
                var contaDestino = await _unitOfWork.Clientes.GetByIdAsync(createTransferenciaDTO.ContaDestinoId);

                if (contaOrigem == null)
                {
                    throw new CustomException("Conta de origem não encontrada.", 400);
                }

                if (contaDestino == null)
                {
                    throw new CustomException("Conta de destino não encontrada.", 400);
                }

                if (contaOrigem.Saldo < createTransferenciaDTO.Valor)
                {
                    throw new CustomException("Saldo insuficiente na conta de origem.", 400);
                }

                contaOrigem.Saldo -= createTransferenciaDTO.Valor;
                contaDestino.Saldo += createTransferenciaDTO.Valor;

                var transferencia = _transferenciaMapper.ConvertToEntityForCreate(createTransferenciaDTO);
                transferencia.Status = StatusTransferenciaEnum.Sucesso; 

                await _unitOfWork.Transferencias.AddAsync(transferencia);
                await _unitOfWork.CompleteAsync(); 

                return _transferenciaMapper.ConvertToDtoForRead(transferencia);
            }
            catch (CustomException ex)
            {
                var transferenciaFalha = _transferenciaMapper.ConvertToEntityForCreate(createTransferenciaDTO);
                transferenciaFalha.Status = StatusTransferenciaEnum.Falha; 
                transferenciaFalha.MensagemErro = ex.Message;

                await _unitOfWork.Transferencias.AddAsync(transferenciaFalha);
                await _unitOfWork.CompleteAsync();

                throw; 
            }
            finally
            {
                _semaphore.Release();
            }

        }
        public async Task<bool> CancelarTransferenciaAsync(Guid id)
        {
            var transferencia = await GetTranferenciaById(id);

            transferencia.Status = StatusTransferenciaEnum.Cancelada;
            await _unitOfWork.Transferencias.UpdateAsync(transferencia);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteTransferenciaAsync(Guid id)
        {
            var transferencia = await GetTranferenciaById(id);

            await _unitOfWork.Transferencias.DeleteAsync(transferencia);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<TransferenciaDTO>> GetHistoricoTransferenciasAsync(Guid contaId)
        {
            if (contaId == Guid.Empty)
                throw new CustomException("Conta inválida.", StatusCodes.Status400BadRequest);

            var transferencias = await _unitOfWork.Transferencias.GetHistoricoPorContaAsync(contaId);

            if (transferencias == null)
                throw new CustomException("Transferencias não encontradas.", StatusCodes.Status404NotFound);

            return transferencias.Select(t => _transferenciaMapper.ConvertToDtoForRead(t));
        }

        private async Task<TransferenciaEntity> GetTranferenciaById(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomException("ID inválido.", StatusCodes.Status400BadRequest);

            var transferencia = await _unitOfWork.Transferencias.GetByIdAsync(id);

            if (transferencia == null)
                throw new CustomException("Transferencia não encontrada.", StatusCodes.Status404NotFound);

            return transferencia;
        }
    }
}
