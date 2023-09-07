using FluentValidation;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;

namespace SkedAuthorization.Application.Services.Validators;

public class SignInDTOValidator : AbstractValidator<SignInDTO>
{
    public SignInDTOValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Empty Email address.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.InvalidEmail));
        RuleFor(x => x.PassHash).NotEmpty().WithMessage("Empty PassHash")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.InvalidPass));
    }
}