using Asp.Versioning;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Transferencia.Application.Dtos.Cliente;
using Transferencia.Application.Dtos.Transferencia;
using Transferencia.Application.Interfaces.Mappers;
using Transferencia.Application.Interfaces.Services;
using Transferencia.Application.Mappers;
using Transferencia.Application.Services;
using Transferencia.Application.Validators.Cliente;
using Transferencia.Application.Validators.Transferencia;
using Transferencia.Core.Interfaces.Persistence.Repositories;
using Transferencia.Persistence;
using Transferencia.Persistence.Data;
using Transferencia.Persistence.Repositories;

namespace Transferencia.Api.Configuration
{
    public static class ServicesExtensions
    {

        public static IServiceCollection AddApiServices(this IServiceCollection services) {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version")
                );
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<ITransferenciaService, TransferenciaService>();
            services.AddScoped<IValidator<CreateClienteDTO>, CreateClienteDTOValidator>();
            services.AddScoped<IValidator<UpdateClienteDTO>, UpdateClienteDTOValidator>();
            services.AddScoped<IValidator<CreateTransferenciaDTO>, CreateTransferenciaDTOValidator>();
            services.AddScoped<IValidator<UpdateTransferenciaDTO>, UpdateTransferenciaDTOValidator>();

            services.AddScoped<IClienteMapper, ClienteMapper>();
            services.AddScoped<ITransferenciaMapper, TransferenciaMapper>();


            return services;
        }

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => 
                options.UseInMemoryDatabase("TransferenciaDb")
                       .EnableSensitiveDataLogging() 
                       .LogTo(Console.WriteLine, LogLevel.Information));

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
