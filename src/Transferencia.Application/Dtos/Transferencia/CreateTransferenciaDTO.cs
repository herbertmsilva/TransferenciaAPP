using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transferencia.Application.Dtos.Transferencia
{
    public class CreateTransferenciaDTO
    {
        public Guid ContaOrigemId { get; set; }
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransferencia { get; set; }
    }
}
