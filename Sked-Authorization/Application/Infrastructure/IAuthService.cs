using Shked.UserDAL.Entities;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;

namespace SkedAuthorization.Application.Infrastructure;
/// <summary>
/// Представляет сервис бизнес-логики авторизации
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Зарегестрировать новую учётную запись
    /// </summary>
    /// <param name="signUpDto"></param>
    /// <returns></returns>
    Task<AuthResult<AuthDTO>> SignUpAsync(SignUpDTO signUpDto);
    /// <summary>
    /// Войти в существующую учётную запись
    /// </summary>
    /// <param name="signInDto"></param>
    /// <returns></returns>
    Task<AuthResult<AuthDTO>> SignInAsync(SignInDTO signInDto);
    /// <summary>
    /// Обновить accessToken
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task<AuthResult<AuthDTO>> RefreshTokenAsync(string refreshToken);
    /// <summary>
    /// Выйти из конкретного сеанса учётной записи
    /// </summary>
    /// <param name="refreshToken">RefreshToken устройства с которого пришёл запрос</param>
    /// <returns></returns>
    Task<AuthResult> Logout(string refreshToken);
    /// <summary>
    /// Выйти сразу из всех устройств, привязанных к учетной записи
    /// </summary>
    /// <param name="id">UserId учетной записи</param>
    /// <returns></returns>
    Task<AuthResult> LogoutFromAll(string id);
}