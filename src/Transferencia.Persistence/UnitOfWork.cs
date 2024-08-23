using Microsoft.EntityFrameworkCore.Storage;
using Transferencia.Core.Interfaces.Persistence.Repositories;
using Transferencia.Persistence.Data;
using Transferencia.Persistence.Repositories;

namespace Transferencia.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IClienteRepository _clientes;
        private ITransferenciaRepository _transferencias;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IClienteRepository Clientes => _clientes ??= new ClienteRepository(_context);
        public ITransferenciaRepository Transferencias => _transferencias ??= new TransferenciaRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
