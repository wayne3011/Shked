using FluentValidation;
using ShkedAuthorization.Application.Data.Responses;
using ShkedAuthorization.Application.Data.DTO;

namespace ShkedAuthorization.Application.Services.Validators;
/// <summary>
/// Валидатор для модели <see cref="SignInDTO"/> входа в учётную запись пользователя 
/// </summary>
public class SignInDTOValidator : AbstractValidator<SignInDTO>
{
    public SignInDTOValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Empty Email address.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmptyEmail));
        RuleFor(x => x.Password).NotEmpty().WithMessage("Empty Password")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmptyPassword));
    }
}