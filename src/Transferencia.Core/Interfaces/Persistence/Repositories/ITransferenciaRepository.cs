using Transferencia.Core.Entities;

namespace Transferencia.Core.Interfaces.Persistence.Repositories
{
    public interface ITransferenciaRepository : IBaseRepository<TransferenciaEntity>
    {
        Task<IEnumerable<TransferenciaEntity>> GetByContaIdAsync(Guid contaId);
        Task<IEnumerable<TransferenciaEntity>> GetHistoricoPorContaAsync(Guid contaId);
    }
}
