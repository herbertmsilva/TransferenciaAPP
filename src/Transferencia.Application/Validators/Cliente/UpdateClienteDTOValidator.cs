using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transferencia.Application.Dtos.Cliente;

namespace Transferencia.Application.Validators.Cliente
{
    public class UpdateClienteDTOValidator : AbstractValidator<UpdateClienteDTO>
    {
        public UpdateClienteDTOValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres.");

            RuleFor(x => x.NumeroConta)
                .NotEmpty().WithMessage("O número da conta é obrigatório.")
                .Length(6, 10).WithMessage("O número da conta deve ter entre 6 e 10 dígitos.");

            RuleFor(x => x.Saldo)
                .GreaterThanOrEqualTo(0).WithMessage("O saldo deve ser maior ou igual a zero.");
        }
    }
}
