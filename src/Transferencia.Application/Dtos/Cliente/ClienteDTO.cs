using System.ComponentModel.DataAnnotations;

namespace Transferencia.Application.Dtos.Cliente
{

    public class ClienteDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string NumeroConta { get; set; }
        public decimal Saldo { get; set; }
        public DateTime DataDeCriacao { get; set; }
    }
    
}
