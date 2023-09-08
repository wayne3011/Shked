using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;
using SkedAuthorization.Application.Extensions;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;

namespace SkedAuthorization.Application.Services.Validators;
using FluentValidation;

public class SignUpDTOValidator : AbstractValidator<SignUpDTO>
{
    public SignUpDTOValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Empty Email argument.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmptyEmail));
        RuleFor(x => x.Email).MustAsync(async (rootObject, member, token) => await userRepository.GetByEmail(member) == null)
            .When(obj => !string.IsNullOrEmpty(obj.Email))
            .WithMessage("This email address is already occupied.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmailOccupied));
        
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Empty Full Name argument.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmptyFullName));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Empty Pass argument.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmptyPassword));

        RuleFor(x => x.Password)
            .Must(x => x.Length > 7 && x.Any(char.IsUpper) && x.Any(char.IsLower) && x.Any(char.IsDigit)).When(obj => !string.IsNullOrEmpty(obj.Password))
            .WithMessage("The password is too simple.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.InvalidPass));
        
        RuleFor(x => x.Group).NotEmpty().WithMessage("Invalid Group argument.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmptyGroup)); //TODO: Group validation
    }
}