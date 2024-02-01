using FluentValidation.Results;
using ShkedAuthorization.Application.Data.Responses;

namespace ShkedAuthorization.Application.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// Заменяет первое вхождение элемента в список на новое значение
    /// </summary>
    /// <param name="list"></param>
    /// <param name="oldItem">Заменяемое значение</param>
    /// <param name="newItem">Новое значение</param>
    public static void ReplaceFirst(this List<string> list, string oldItem, string newItem)
    {
        var index = list.IndexOf(oldItem);
        list[index] = newItem;
    }
    /// <summary>
    /// Приводит список ошибок из <see cref="FluentValidation"/> к списку <see cref="ValidationError"/>
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<ValidationError> ToValidationErrorsList(this IEnumerable<ValidationFailure> list)
    {
        return list.Select(error => new ValidationError() { ErrorCode = int.Parse(error.ErrorCode), ErrorMessage = error.ErrorMessage }).ToList();
    }
}