using Microsoft.EntityFrameworkCore;
using Transferencia.Core.Entities;

namespace Transferencia.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ClienteEntity> Clientes { get; set; }
        public DbSet<TransferenciaEntity> Transferencias { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
        }
    }
}
