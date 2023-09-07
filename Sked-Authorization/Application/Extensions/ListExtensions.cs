using FluentValidation.Results;
using SkedAuthorization.Application.Data.Responses;

namespace SkedAuthorization.Application.Extensions;

public static class ListExtensions
{
    public static void ReplaceFirst(this List<string> list, string oldItem, string newItem)
    {
        var index = list.IndexOf(oldItem);
        list[index] = newItem;
    }
    public static List<ValidationError> ToValidationErrorsList(this IEnumerable<ValidationFailure> list)
    {
        return list.Select(error => new ValidationError() { ErrorCode = int.Parse(error.ErrorCode), ErrorMessage = error.ErrorMessage }).ToList();
    }
}