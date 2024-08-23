using Microsoft.EntityFrameworkCore.Storage;

namespace Transferencia.Core.Interfaces.Persistence.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IClienteRepository Clientes { get; }
        ITransferenciaRepository Transferencias { get; }
        Task<int> CompleteAsync();  
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
