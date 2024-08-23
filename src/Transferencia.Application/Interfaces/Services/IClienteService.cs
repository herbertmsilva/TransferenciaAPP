using Transferencia.Application.Dtos.Cliente;

namespace Transferencia.Application.Interfaces.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> GetAllClientesAsync();
        Task<ClienteDTO> GetClienteByIdAsync(Guid id);
        Task<ClienteDTO> GetClienteByNumeroContaAsync(string numeroConta);
        Task<ClienteDTO> AddClienteAsync(CreateClienteDTO createClienteDTO);
        Task<bool> UpdateClienteAsync(Guid id, UpdateClienteDTO updateClienteDTO);
        Task<bool> DeleteClienteAsync(Guid id);
    }
}
