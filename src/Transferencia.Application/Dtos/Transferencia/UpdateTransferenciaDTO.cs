using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transferencia.Application.Dtos.Transferencia
{
    public class UpdateTransferenciaDTO
    {
        public Guid Id { get; set; }
        public Guid ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransferencia { get; set; }
    }
}
