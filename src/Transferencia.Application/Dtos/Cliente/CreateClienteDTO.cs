using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transferencia.Application.Dtos.Cliente
{
    public class CreateClienteDTO
    {
        public string Nome { get; set; }
        public string NumeroConta { get; set; }
        public decimal Saldo { get; set; }
    }
}
