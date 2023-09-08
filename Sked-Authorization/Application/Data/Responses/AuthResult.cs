using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace SkedAuthorization.Application.Data.Responses;

public class AuthResult<T>
{
    public T? Value { get; }
    public List<ValidationError>? ValidateErrors { get; }
    public AuthResult(T? value, List<ValidationError> validateErrors)
    {
        Value = value;
        ValidateErrors = validateErrors;
    }
}

public class AuthResult
{
    public AuthResultCode Code { get; }

    public AuthResult(AuthResultCode code)
    {
        Code = code;
    }
}

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