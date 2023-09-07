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
            .WithErrorCode(Convert.ToString((int)AuthResultCode.InvalidEmail));
        RuleFor(x => x.Email).MustAsync(async (rootObject, member, token) => await userRepository.GetByEmail(member) == null)
            .WithMessage("This email address is already occupied.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmailOccupied));
        
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Empty Full Name argument.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.EmptyFullName));
        
        RuleFor(x => x.PassHash)
            .Must((rootObj,s) => !string.IsNullOrEmpty(s) && s.Length > 7 && s != rootObj.Email && s.Any(char.IsLower) && s.Any(char.IsUpper) && s.Any(char.IsDigit))
            .WithMessage("Invalid Pass Hash argument.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.InvalidPass));
        
        RuleFor(x => x.Group).NotEmpty().WithMessage("Invalid Group argument.")
            .WithErrorCode(Convert.ToString((int)AuthResultCode.InvalidGroup)); //TODO: Group validation
    }
}