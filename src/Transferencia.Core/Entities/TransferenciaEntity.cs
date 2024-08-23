using Transferencia.Core.Enums;

namespace Transferencia.Core.Entities
{
    public class TransferenciaEntity : BaseEntity
    {
        public Guid ContaOrigemId { get; set; }
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransferencia { get; set; }
        public StatusTransferenciaEnum Status { get; set; }  
        public string? MensagemErro { get; set; } 
        public DateTime DataCriacao { get; set; }
        public ClienteEntity ContaOrigem { get; set; }
        public ClienteEntity ContaDestino { get; set; }


    }
}