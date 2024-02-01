using System.Text.RegularExpressions;
using FluentValidation;
using ShkedAuthorization.Application.Data.Responses;

namespace ShkedAuthorization.Application.Extensions;

public static class ValidationRulesExtensions
{
    public static IRuleBuilderOptions<T, string> ComplexPass<T>(this IRuleBuilderOptions<T, string> builder)
    {
        return builder.Must(s => s.Length > 7 && s.Any(char.IsLower) && s.Any(char.IsUpper) && s.Any(char.IsDigit));
    }
}
