using Transferencia.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Transferencia.Persistence.Data;
using Transferencia.Core.Interfaces.Persistence.Repositories;

namespace Transferencia.Persistence.Repositories
{
    public class TransferenciaRepository : BaseRepository<TransferenciaEntity>, ITransferenciaRepository
    {
        public TransferenciaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TransferenciaEntity>> GetByContaIdAsync(Guid contaId)
        => await _context.Set<TransferenciaEntity>()
                .Where(t => t.ContaOrigemId == contaId || t.ContaDestinoId == contaId)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();

        public async Task<IEnumerable<TransferenciaEntity>> GetHistoricoPorContaAsync(Guid contaId)
        => await _context.Set<TransferenciaEntity>()
                .Where(t => t.ContaOrigemId == contaId || t.ContaDestinoId == contaId)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
    }
}
