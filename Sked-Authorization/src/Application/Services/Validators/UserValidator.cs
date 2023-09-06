using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;

namespace SkedAuthorization.Application.Services.Validators;
using FluentValidation;

public class UserValidator : AbstractValidator<SignUpDTO>
{
    public UserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Empty Email argument.");
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Empty Full Name argument.");
        RuleFor(x => x.PassHash).NotEmpty().WithMessage("Empty Pass Hash argument.");
        RuleFor(x => x.Group).NotEmpty().WithMessage("Empty Group argument.");
    }
}