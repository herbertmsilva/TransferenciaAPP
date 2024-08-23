using Transferencia.Application.Dtos.Transferencia;

namespace Transferencia.Application.Interfaces.Services
{
    public interface ITransferenciaService
    {
        Task<IEnumerable<TransferenciaDTO>> GetAllTransferenciasAsync();
        Task<TransferenciaDTO> GetTransferenciaByIdAsync(Guid id);
        Task<TransferenciaDTO> RealizarTransferenciaAsync(CreateTransferenciaDTO createTransferenciaDTO);
        Task<bool> CancelarTransferenciaAsync(Guid id);
        Task<bool> DeleteTransferenciaAsync(Guid id);
        Task<IEnumerable<TransferenciaDTO>> GetHistoricoTransferenciasAsync(Guid contaId);
    }
}
