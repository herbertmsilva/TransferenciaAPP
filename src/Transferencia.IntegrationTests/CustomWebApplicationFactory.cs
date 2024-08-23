using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Transferencia.Core.Entities;
using Transferencia.Core.Enums;
using Transferencia.Persistence.Data;


namespace Transferencia.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();
                    
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    SeedDatabase(db);

                }
            });
            builder.UseEnvironment("Development");
        }

        private void SeedDatabase(AppDbContext context)
        {

            var clienteOrigem = new ClienteEntity
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste 1",
                NumeroConta = "123456",
                Saldo = 1000,
            };

            var clienteDestino = new ClienteEntity
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste 2",
                NumeroConta = "654321",
                Saldo = 2000,
            };

            context.Clientes.Add(clienteOrigem);
            context.Clientes.Add(clienteDestino);

            var transferencia1 = new TransferenciaEntity
            {
                Id = Guid.NewGuid(),
                ContaOrigemId = clienteOrigem.Id,
                ContaDestinoId = clienteDestino.Id,
                Valor = 1000,
                DataTransferencia = DateTime.Now.AddDays(-1),
                Status = StatusTransferenciaEnum.Sucesso
            };

            var transferencia2 = new TransferenciaEntity
            {
                Id = Guid.NewGuid(),
                ContaOrigemId = clienteOrigem.Id,
                ContaDestinoId = clienteDestino.Id,
                Valor = 500,
                DataTransferencia = DateTime.Now.AddDays(-2),
                Status = StatusTransferenciaEnum.Sucesso
            };

            context.Transferencias.Add(transferencia1);
            context.Transferencias.Add(transferencia2);

            context.SaveChanges();

            context.SaveChanges();
        }
    }
}