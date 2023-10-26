using System.Text.Json.Serialization;
using SkedAuthorization.Application.Infrastructure;
using ThirdParty.Json.LitJson;

namespace SkedAuthorization.Application.Data.Responses;
/// <summary>
/// Представляет результат действия авторизации для <see cref="IAuthService"/>
/// </summary>
/// <typeparam name="T">Тип возвращаемого значение действия авторизации</typeparam>
public class AuthResult<T>
{
    /// <summary>
    /// Возвращаемое значения действия авторизации
    /// </summary>
    public T? Value { get; }
    /// <summary>
    /// Список ошибок действия авторизации
    /// </summary>
    public List<ValidationError>? ValidateErrors { get; }
    public AuthResult(T? value, List<ValidationError> validateErrors)
    {
        Value = value;
        ValidateErrors = validateErrors;
    }
}
/// <summary>
/// Представляет возвращаемый код результата действия авторизации для <see cref="IAuthService"/>
/// </summary>
public class AuthResult
{
    public AuthResultCode Code { get; }

    public AuthResult(AuthResultCode code)
    {
        Code = code;
    }
}
/// <summary>
/// Ошибка валидации данных авторизации
/// </summary>
public class ValidationError
{
    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; set; }
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }
}
public enum AuthResultCode
{
    EmailOccupied,
    Ok,
    InvalidEmail,
    EmptyEmail,
    InvalidPass,
    InvalidUserId,
    InvalidRefreshToken,
    EmptyFullName,
    EmptyGroup,
    InvalidCredentials,
    EmptyPassword,
}