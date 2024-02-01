using ShkedAuthorization.Application.Data.DTO;

namespace ShkedAuthorization.Application.Infrastructure;
/// <summary>
/// Представляет собой менеджер токенов авторизации
/// </summary>
public interface ITokenManager
{
    /// <summary>
    /// Выдать новые токены доступа пользователю
    /// </summary>
    /// <param name="id">UserId учётной записи пользователя</param>
    /// <returns></returns>
    AuthDTO IssueToken(string id);
    /// <summary>
    /// Получить UserId из Refresh-токена
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    string? GetUserId(string refreshToken);
}