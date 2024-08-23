using Microsoft.EntityFrameworkCore;
using Transferencia.Core.Entities;
using Transferencia.Core.Interfaces.Persistence.Repositories;
using Transferencia.Persistence.Data;

namespace Transferencia.Persistence.Repositories
{
    public class ClienteRepository : BaseRepository<ClienteEntity>, IClienteRepository
    {
        public ClienteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ClienteEntity> GetByNumeroContaAsync(string numeroConta)
        =>  await _context.Clientes.SingleOrDefaultAsync(c => c.NumeroConta == numeroConta);
    }
}
