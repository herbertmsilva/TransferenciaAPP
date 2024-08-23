using Transferencia.Core.Entities;

namespace Transferencia.Core.Interfaces.Persistence.Repositories
{
    public interface IClienteRepository : IBaseRepository<ClienteEntity>
    {
        Task<ClienteEntity> GetByNumeroContaAsync(string numeroConta);
    }
}
