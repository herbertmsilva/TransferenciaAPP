using System.ComponentModel.DataAnnotations;

namespace Transferencia.Core.Entities
{
    public class ClienteEntity : BaseEntity
    {
        public string Nome { get; set; }
        public string NumeroConta { get; set; }
        public decimal Saldo { get; set; }
        public int Version {  get; set; }
    }
}