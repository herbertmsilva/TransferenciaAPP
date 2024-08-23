using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transferencia.Application.Dtos.Cliente;
using Transferencia.Core.Entities;

namespace Transferencia.Application.Interfaces.Mappers
{
    public interface IClienteMapper
    {
        ClienteEntity ConvertToEntityForCreate(CreateClienteDTO dto);
        ClienteDTO ConvertToDtoForRead(ClienteEntity entity);
        ClienteEntity ConvertToEntityForUpdate(UpdateClienteDTO dto, ClienteEntity existingEntity);
    }
}
