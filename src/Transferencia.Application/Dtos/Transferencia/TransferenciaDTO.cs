using Transferencia.Core.Enums;

namespace Transferencia.Application.Dtos.Transferencia
{
    public class TransferenciaDTO
    {
        public Guid Id { get; set; }
        public Guid ContaOrigemId { get; set; }
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransferencia { get; set; }
        public DateTime DataCriacao { get; set; }
        public StatusTransferenciaEnum Status { get; set; }
        public string MensagemErro { get; set; }
    }
}
