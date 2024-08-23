using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transferencia.Application.Dtos.Transferencia;

namespace Transferencia.Application.Validators.Transferencia
{
    public class CreateTransferenciaDTOValidator : AbstractValidator<CreateTransferenciaDTO>
    {
        public CreateTransferenciaDTOValidator()
        {
            RuleFor(x => x.ContaOrigemId)
                .NotEmpty().WithMessage("A conta de origem é obrigatória.");

            RuleFor(x => x.ContaDestinoId)
                .NotEmpty().WithMessage("A conta de destino é obrigatória.");

            RuleFor(x => x.Valor)
                .GreaterThan(0).WithMessage("O valor da transferência deve ser maior que zero.");

            RuleFor(x => x.DataTransferencia)
                .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("A data da transferência não pode ser no passado.");
        }
    }
}
