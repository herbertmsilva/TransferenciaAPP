using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transferencia.Application.Dtos.Cliente;
using Transferencia.Application.Exceptions;
using Transferencia.Application.Interfaces.Mappers;
using Transferencia.Application.Interfaces.Services;
using Transferencia.Core.Entities;
using Transferencia.Core.Interfaces.Persistence.Repositories;

namespace Transferencia.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IValidator<CreateClienteDTO> _createValidator;
        private readonly IValidator<UpdateClienteDTO> _updateValidator;
        private readonly IClienteMapper _clienteMapper;

        public ClienteService(
            IClienteRepository clienteRepository,
            IValidator<CreateClienteDTO> createValidator,
            IValidator<UpdateClienteDTO> updateValidator,
            IClienteMapper clienteMapper)
        {
            _clienteRepository = clienteRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _clienteMapper = clienteMapper;
        }

        public async Task<IEnumerable<ClienteDTO>> GetAllClientesAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return clientes.Select(c => _clienteMapper.ConvertToDtoForRead(c));
        }

        public async Task<ClienteDTO> GetClienteByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomException("ID inválido.", StatusCodes.Status400BadRequest);

            var cliente = await _clienteRepository.GetByIdAsync(id);

            if (cliente == null)
                throw new CustomException("Cliente não encontrado.", StatusCodes.Status404NotFound);

            return _clienteMapper.ConvertToDtoForRead(cliente);
        }

        public async Task<ClienteDTO> GetClienteByNumeroContaAsync(string numeroConta)
        {
            if (string.IsNullOrEmpty(numeroConta))
                throw new CustomException("Número da conta inválido.", StatusCodes.Status400BadRequest);

            var cliente = await _clienteRepository.GetByNumeroContaAsync(numeroConta);
            if (cliente == null)
                throw new CustomException("Cliente não encontrado pelo número da conta informada.", StatusCodes.Status404NotFound);

            return _clienteMapper.ConvertToDtoForRead(cliente);
        }

        public async Task<ClienteDTO> AddClienteAsync(CreateClienteDTO createClienteDTO)
        {
            var validationResult = await _createValidator.ValidateAsync(createClienteDTO);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var clienteExistente = await _clienteRepository.GetByNumeroContaAsync(createClienteDTO.NumeroConta);
            if (clienteExistente != null)
            {
                throw new CustomException("O número da conta já está em uso.", StatusCodes.Status400BadRequest);
            }

            var cliente = _clienteMapper.ConvertToEntityForCreate(createClienteDTO);

            await _clienteRepository.AddAsync(cliente);
            await _clienteRepository.SaveAsync();

            return _clienteMapper.ConvertToDtoForRead(cliente);
        }

        public async Task<bool> UpdateClienteAsync(Guid id, UpdateClienteDTO updateClienteDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateClienteDTO);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) return false;

            cliente.Nome = updateClienteDTO.Nome;
            cliente.NumeroConta = updateClienteDTO.NumeroConta;
            cliente.Saldo = updateClienteDTO.Saldo;
            cliente.DataAtualizacao = DateTime.UtcNow;

            await _clienteRepository.UpdateAsync(cliente);
            await _clienteRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteClienteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomException("ID inválido.", StatusCodes.Status400BadRequest);

            var cliente = await _clienteRepository.GetByIdAsync(id);

            if (cliente == null) 
                throw new CustomException("Cliente não encontrado.", StatusCodes.Status404NotFound);


            await _clienteRepository.DeleteAsync(cliente);
            await _clienteRepository.SaveAsync();
            return true;
        }
    }
}
