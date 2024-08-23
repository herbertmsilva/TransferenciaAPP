using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transferencia.Application.Dtos.Transferencia;
using Transferencia.Application.Interfaces.Mappers;
using Transferencia.Core.Entities;
using Transferencia.Core.Enums;

namespace Transferencia.Application.Mappers
{
    public class TransferenciaMapper : ITransferenciaMapper
    {
        public TransferenciaDTO ConvertToDtoForRead(TransferenciaEntity entity) =>
            new TransferenciaDTO
            {
                Id = entity.Id,
                ContaOrigemId = entity.ContaOrigemId,
                ContaDestinoId = entity.ContaDestinoId,
                Valor = entity.Valor,
                DataTransferencia = entity.DataTransferencia,
                DataCriacao = entity.DataCriacao
            };

        public TransferenciaEntity ConvertToEntityForCreate(CreateTransferenciaDTO dto) =>
            new TransferenciaEntity
            {
                ContaOrigemId = dto.ContaOrigemId,
                ContaDestinoId = dto.ContaDestinoId,
                Valor = dto.Valor,
                DataTransferencia = dto.DataTransferencia,
                DataCriacao = DateTime.UtcNow,
                Status = StatusTransferenciaEnum.Sucesso
            };

        public TransferenciaEntity ConvertToEntityForUpdate(UpdateTransferenciaDTO dto, TransferenciaEntity existingEntity)
        {
            if (dto.ContaDestinoId != Guid.Empty)
            {
                existingEntity.ContaDestinoId = dto.ContaDestinoId;
            }

            if (dto.Valor != default)
            {
                existingEntity.Valor = dto.Valor;
            }

            if (dto.DataTransferencia != default)
            {
                existingEntity.DataTransferencia = dto.DataTransferencia;
            }

            existingEntity.DataAtualizacao = DateTime.UtcNow;

            return existingEntity;
        }
    }
}
