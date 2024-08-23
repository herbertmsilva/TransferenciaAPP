using System.Security.Cryptography;
using Transferencia.Application.Dtos.Cliente;
using Transferencia.Application.Interfaces.Mappers;
using Transferencia.Core.Entities;

namespace Transferencia.Application.Mappers
{
    public class ClienteMapper : IClienteMapper
    {
        public ClienteEntity ConvertToEntityForCreate(CreateClienteDTO dto) =>
            new ClienteEntity
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                NumeroConta = dto.NumeroConta,
                Saldo = dto.Saldo,
                DataInclusao = DateTime.UtcNow
            };

        public ClienteDTO ConvertToDtoForRead(ClienteEntity entity) =>
                new ClienteDTO
                {
                    Id = entity.Id,
                    Nome = entity.Nome,
                    NumeroConta = entity.NumeroConta,
                    Saldo = entity.Saldo,
                    DataDeCriacao = entity.DataInclusao
                };

        public ClienteEntity ConvertToEntityForUpdate(UpdateClienteDTO dto, ClienteEntity existingEntity)
        {
            if (!string.IsNullOrEmpty(dto.Nome))
            {
                existingEntity.Nome = dto.Nome;
            }

            if (!string.IsNullOrEmpty(dto.NumeroConta))
            {
                existingEntity.NumeroConta = dto.NumeroConta;
            }

            if (dto.Saldo != existingEntity.Saldo)
            {
                existingEntity.Saldo = dto.Saldo;
            }

            existingEntity.DataAtualizacao = DateTime.UtcNow;

            return existingEntity;
        }
    }
}
