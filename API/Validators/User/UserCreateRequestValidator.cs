using FluentValidation;
using SNI_Events.Application.Dtos.User;

namespace SNI_Events.API.Validators.User
{
    public class UserCreateRequestValidator : AbstractValidator<UserCreateRequestDto>
    {
        public UserCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail está em formato inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");

            //RuleFor(x => x.PhoneNumber)
            //    .NotEmpty().WithMessage("O telefone é obrigatório.")
            //    .Matches(@"^\d{10,11}$").WithMessage("O telefone deve conter 10 ou 11 dígitos numéricos.");

            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("O CPF é obrigatório.")
                .Length(11).WithMessage("O CPF deve conter 11 dígitos.")
                .Matches(@"^\d{11}$").WithMessage("CPF deve conter apenas números.");
        }
    }
}
