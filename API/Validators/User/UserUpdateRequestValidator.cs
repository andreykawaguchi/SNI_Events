using FluentValidation;
using SNI_Events.Application.Dtos.User;

namespace SNI_Events.API.Validators.User
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequestDto>
    {
        public UserUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("E-mail inválido.");

            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("O CPF é obrigatório.")
                .Length(11).WithMessage("O CPF deve conter 11 dígitos.")
                .Matches(@"^\d{11}$").WithMessage("CPF deve conter apenas números.");
        }
    }
}
