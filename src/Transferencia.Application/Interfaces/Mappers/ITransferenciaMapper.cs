using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transferencia.Application.Dtos.Transferencia;
using Transferencia.Core.Entities;

namespace Transferencia.Application.Interfaces.Mappers
{
    public interface ITransferenciaMapper
    {
        TransferenciaEntity ConvertToEntityForCreate(CreateTransferenciaDTO dto);
        TransferenciaEntity ConvertToEntityForUpdate(UpdateTransferenciaDTO dto, TransferenciaEntity existingEntity);
        TransferenciaDTO ConvertToDtoForRead(TransferenciaEntity entity);
    }
}
