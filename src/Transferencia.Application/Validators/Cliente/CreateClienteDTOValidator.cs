using FluentValidation;
using Transferencia.Application.Dtos.Cliente;

namespace Transferencia.Application.Validators.Cliente
{
    public class CreateClienteDTOValidator : AbstractValidator<CreateClienteDTO>
    {
        public CreateClienteDTOValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
                .Length(2, 100).WithMessage("O nome do cliente deve ter entre 2 e 100 caracteres.");

            RuleFor(x => x.NumeroConta)
                .NotEmpty().WithMessage("O número da conta é obrigatório.")
                .Length(6, 10).WithMessage("O número da conta deve ter entre 6 e 10 dígitos.");

            RuleFor(x => x.Saldo)
                .GreaterThanOrEqualTo(0).WithMessage("O saldo do cliente não pode ser negativo.");
        }
    }
}