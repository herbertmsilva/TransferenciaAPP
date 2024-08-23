using Transferencia.Core.Entities;
using Transferencia.Core.Interfaces.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Transferencia.Persistence.Data;

namespace Transferencia.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            entity.DataInclusao = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            entity.DataAtualizacao = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }
        
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}